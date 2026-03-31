using UnityEngine;

public enum NPCState { Idle, Playing, Leaving }

public class NPC : MonoBehaviour
{
    [Header("Base Stats")]
    public float patienceTotal = 60f;
    public float baseBetAmount = 10f;
    public float basePatienceWinGain = 0.5f;
    public float playInterval = 2f;

    public NPCState State { get; private set; } = NPCState.Idle;

    private Machine _assignedMachine;
    private float _patienceMachine;
    private float _playTimer;

    private int _totalPlays = 0;
    private int _actualWins = 0;
    private float _expectedWins = 0f; // accumulé par machine

    private float BetAmount => baseBetAmount * (_assignedMachine?.betAmountMultiplier ?? 1f);
    private float PatienceWinGain => basePatienceWinGain * (_assignedMachine?.patienceWinGainMultiplier ?? 1f);

    void Update()
    {
        patienceTotal -= Time.deltaTime;

        if (State == NPCState.Playing || State == NPCState.Leaving)
        {
            _patienceMachine -= Time.deltaTime;
            _playTimer -= Time.deltaTime;

            if (_playTimer <= 0f && State == NPCState.Playing)
            {
                _playTimer = playInterval;
                Play();
            }

            if (_patienceMachine <= 0f)
                LeaveMachine();
        }

        if (patienceTotal <= 0f && State != NPCState.Leaving)
            StartLeaving();
    }

    public void AssignMachine(Machine machine)
    {
        _assignedMachine = machine;
        _patienceMachine = machine.sessionDuration;
        _playTimer = playInterval;
        State = NPCState.Playing;
    }

    private void Play()
    {
        bool win = _assignedMachine.Play();
        _totalPlays++;
        _expectedWins += _assignedMachine.payoutRate;

        if (win)
        {
            _actualWins++;
            patienceTotal += PatienceWinGain;
            CurrencyManager.Instance.AddMoney(-BetAmount);
        }
        else
        {
            CurrencyManager.Instance.AddMoney(BetAmount);
        }
    }

    private void LeaveMachine()
    {
        _assignedMachine.FreeSlot(this);
        _assignedMachine = null;

        if (State == NPCState.Leaving)
            NPCManagementSystem.Instance.OnNPCLeaving(this);
        else
        {
            State = NPCState.Idle;
            NPCManagementSystem.Instance.OnNPCBecameIdle(this);
        }
    }

    private void StartLeaving()
    {
        State = NPCState.Leaving;
        if (_assignedMachine == null)
            NPCManagementSystem.Instance.OnNPCLeaving(this);
        // Si sur machine → attend que patienceMachine tombe à 0
    }

    public float ComputeSatisfaction()
    {
        if (_totalPlays == 0) 
        {
            return 0f; // n'a jamais joué → pire cas
        }
        if (_expectedWins == 0f) return 0f;
        return Mathf.Clamp01(_actualWins / _expectedWins);
    }
}