using UnityEngine;

public enum NPCState { Idle, Playing, Leaving, Walking }

public class NPC : MonoBehaviour
{
    [Header("Data")]
    public NPCData data;

    public NPCState State { get; private set; } = NPCState.Idle;
    public bool MovingRight { get; private set; }
    public float PatienceMachineRemaining => _patienceMachine;

    private NPCAnimator _animator;
    private Machine _assignedMachine;
    private float _patienceTotal;
    private float _patienceMachine;
    private float _playTimer;

    private int _totalPlays = 0;
    private int _actualWins = 0;
    private float _expectedWins = 0f;

    private float BetAmount => data.baseBetAmount * (_assignedMachine != null ? _assignedMachine.data.betAmountMultiplier : 1f);
    private float PatienceWinGain => data.basePatienceWinGain * (_assignedMachine != null ? _assignedMachine.data.patienceWinGainMultiplier : 1f);

    void Awake()
    {
        _animator = GetComponentInChildren<NPCAnimator>();
    }

    void Start()
    {
        _patienceTotal = data.patienceTotal;
    }

    void Update()
    {
        _patienceTotal -= Time.deltaTime;

        if (State == NPCState.Playing || State == NPCState.Leaving)
        {
            _patienceMachine -= Time.deltaTime;
            _playTimer -= Time.deltaTime;

            if (_playTimer <= 0f && State == NPCState.Playing)
            {
                _playTimer = data.playInterval;
                Play();
            }

            if (_patienceMachine <= 0f)
                LeaveMachine();
        }

        if (_patienceTotal <= 0f && State != NPCState.Leaving)
            StartLeaving();
    }

    public void AssignMachine(Machine machine)
    {
        _assignedMachine = machine;
        _patienceMachine = machine.data.sessionDuration;
        _playTimer = data.playInterval;
        State = NPCState.Playing;
    }

    private void Play()
    {
        var (win, amount) = _assignedMachine.Play(BetAmount);
        _totalPlays++;
        _expectedWins += _assignedMachine.data.payoutRate;

        if (win)
        {
            _actualWins++;
            _patienceTotal += PatienceWinGain;
            CurrencyManager.Instance.AddMoney(-amount); // casino paye le NPC
            _animator?.TriggerWin();
        }
        else
        {
            CurrencyManager.Instance.AddMoney(amount); // casino encaisse la mise
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
        MovingRight = transform.position.x < 0f;
        if (_assignedMachine == null)
            NPCManagementSystem.Instance.OnNPCLeaving(this);
        // Si sur machine → attend que patienceMachine tombe à 0
    }

    public float ComputeSatisfaction()
    {
        if (_totalPlays == 0)
            return 0f;
        if (_expectedWins == 0f) return 0f;
        return Mathf.Clamp01(_actualWins / _expectedWins);
    }
}
