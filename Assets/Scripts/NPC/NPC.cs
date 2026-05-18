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
    private SpriteRenderer _sr;
    private Machine _assignedMachine;
    private float _patienceTotal;
    private float _patienceMachine;
    private float _playTimer;

    // Mouvement
    private Vector2 _spawnPosition;
    private Vector2 _walkTarget;
    private bool _headingToMachine;   // true = trajet vers machine (patience suspendue)
    private float _wanderPauseTimer;

    private int _totalPlays = 0;
    private int _actualWins = 0;
    private float _expectedWins = 0f;
    private float _sessionNetCasino = 0f;

    private float BetAmount => data.baseBetAmount * (_assignedMachine != null ? _assignedMachine.data.betAmountMultiplier : 1f);
    private float PatienceWinGain => data.basePatienceWinGain * (_assignedMachine != null ? _assignedMachine.data.patienceWinGainMultiplier : 1f);

    void Awake()
    {
        _animator = GetComponentInChildren<NPCAnimator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        _spawnPosition = transform.position;
        _patienceTotal = data.patienceTotal;
        if (!_headingToMachine)
            PickWanderTarget();
    }

    void Update()
    {
        _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

        // La patience totale est suspendue pendant le trajet vers une machine
        if (!_headingToMachine)
            _patienceTotal -= Time.deltaTime;

        switch (State)
        {
            case NPCState.Idle:
                UpdateWander();
                break;
            case NPCState.Walking:
                UpdateWalk();
                break;
            case NPCState.Playing:
                UpdatePlaying();
                break;
            case NPCState.Leaving:
                // Si encore sur machine : on attend la fin de session avant de partir
                if (_assignedMachine != null)
                    UpdatePlaying();
                else
                    UpdateLeavingWalk();
                break;
        }

        if (_patienceTotal <= 0f && State != NPCState.Leaving)
            StartLeaving();
    }

    // ── Divagation aléatoire ────────────────────────────────────────────────

    private void UpdateWander()
    {
        _wanderPauseTimer -= Time.deltaTime;
        if (_wanderPauseTimer <= 0f)
            PickWanderTarget();
    }

    private void PickWanderTarget()
    {
        Vector2 offset = Random.insideUnitCircle * data.wanderRadius;
        _walkTarget = _spawnPosition + offset;

        Bounds b = NPCManagementSystem.WanderBounds;
        if (b.size != Vector3.zero)
        {
            Vector2 half = _sr != null ? (Vector2)_sr.bounds.extents : Vector2.zero;
            _walkTarget.x = Mathf.Clamp(_walkTarget.x, b.min.x + half.x, b.max.x - half.x);
            _walkTarget.y = Mathf.Clamp(_walkTarget.y, b.min.y + half.y, b.max.y - half.y);
        }

        State = NPCState.Walking;
    }

    // ── Déplacement en ligne droite ─────────────────────────────────────────

    private void UpdateWalk()
    {
        MoveToward(_walkTarget);

        if (Vector2.Distance(transform.position, _walkTarget) < 0.05f)
        {
            transform.position = new Vector3(_walkTarget.x, _walkTarget.y, transform.position.z);
            OnReachedTarget();
        }
    }

    private void OnReachedTarget()
    {
        if (_headingToMachine)
        {
            BeginPlaying();
        }
        else
        {
            State = NPCState.Idle;
            _wanderPauseTimer = Random.Range(data.wanderPauseMin, data.wanderPauseMax);
        }
    }

    private void MoveToward(Vector2 target)
    {
        Vector2 pos = transform.position;
        Vector2 dir = target - pos;
        float dist = dir.magnitude;
        if (dist < 0.001f) return;

        MovingRight = dir.x > 0f;
        float step = Mathf.Min(data.walkSpeed * Time.deltaTime, dist);
        transform.position += new Vector3(dir.x / dist * step, dir.y / dist * step, 0f);
    }

    // ── Assignation machine ─────────────────────────────────────────────────

    public void AssignMachine(Machine machine)
    {
        _assignedMachine = machine;
        _patienceMachine = machine.SessionDuration;
        _playTimer = data.playInterval;
        _headingToMachine = true;
        _walkTarget = machine.GetStandPosition();
        State = NPCState.Walking;
        _sessionNetCasino = 0f;
    }

    private void BeginPlaying()
    {
        _headingToMachine = false;
        State = NPCState.Playing;
    }

    // ── Session de jeu ──────────────────────────────────────────────────────

    private void UpdatePlaying()
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

    private void Play()
    {
        float bet = BetAmount;
        var (win, amount) = _assignedMachine.Play(bet);
        _totalPlays++;
        _expectedWins += _assignedMachine.GetFairPayoutRate();

        float casinoNet = win
            ? bet * (1f - _assignedMachine.data.winMultiplier) * CurrencyManager.Instance.revenueMultiplier
            : bet * CurrencyManager.Instance.revenueMultiplier;
        _sessionNetCasino += casinoNet;

        if (win)
        {
            _actualWins++;
            _patienceTotal += PatienceWinGain;
            CurrencyManager.Instance.AddMoney(-amount);
            if (_animator != null) _animator.TriggerWin();
        }
        else
        {
            CurrencyManager.Instance.AddMoney(amount);
        }

        CasinoLogger.LogTransaction(gameObject.name, _assignedMachine.name, _assignedMachine.Line?.name ?? "?", bet, win, amount, casinoNet);
    }

    private void LeaveMachine()
    {
        CasinoLogger.LogSessionSummary(gameObject.name, _assignedMachine.name, _assignedMachine.Line?.name ?? "?", _totalPlays, _actualWins, _sessionNetCasino);
        _assignedMachine.FreeSlot(this);
        _assignedMachine = null;

        if (State == NPCState.Leaving)
        {
            // Patience épuisée : commence la marche vers la sortie
            SetupExitWalk();
        }
        else
        {
            State = NPCState.Idle;
            _wanderPauseTimer = Random.Range(data.wanderPauseMin, data.wanderPauseMax);
            NPCManagementSystem.Instance.OnNPCBecameIdle(this);
        }
    }

    // ── Départ du casino ────────────────────────────────────────────────────

    private void StartLeaving()
    {
        State = NPCState.Leaving;
        _headingToMachine = false;

        if (_assignedMachine == null)
            SetupExitWalk();
        // Sinon : on attend que la session machine se termine (LeaveMachine appellera SetupExitWalk)
    }

    private void SetupExitWalk()
    {
        // Marche vers le bord de l'écran du côté le plus proche
        bool goRight = transform.position.x >= 0f;
        MovingRight = goRight;
        _walkTarget = new Vector2(goRight ? 20f : -20f, transform.position.y);
    }

    private void UpdateLeavingWalk()
    {
        MoveToward(_walkTarget);
        if (Mathf.Abs(transform.position.x) > 12f)
            NPCManagementSystem.Instance.OnNPCLeaving(this);
    }

    public float ComputeSatisfaction()
    {
        if (_totalPlays == 0) return 0f;
        if (_expectedWins == 0f) return 0f;
        return Mathf.Clamp01(_actualWins / _expectedWins);
    }
}
