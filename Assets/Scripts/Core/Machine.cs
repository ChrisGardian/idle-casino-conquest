using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Machine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    public MachineData data;

    [Header("NPC Stand Point")]
    [Tooltip("Position where the NPC stands to play. If empty, defaults to machine position - 0.6 on Y.")]
    [SerializeField] private Transform _standPoint;

    public bool HasFreeSlot => _occupants.Count < data.totalSlots + (Line != null ? Line.BonusSlotsPerMachine : 0);
    public float SessionDuration => data.sessionDuration + (Line != null ? Line.BonusSessionDuration : 0f);
    public IReadOnlyList<NPC> Occupants => _occupants;
    public float TotalRevenue => _totalRevenue;
    public float RevenuePerMinute => _trackingTime > 1f ? _totalRevenue / _trackingTime * 60f : 0f;
    public MachineLine Line { get; private set; }
    public bool IsLocked { get; private set; } = false;

    private readonly List<NPC> _occupants = new();
    private float _totalRevenue = 0f;
    private float _trackingTime = 0f;
    private MachineInfoDisplay _infoDisplay;
    private SpriteRenderer _spriteRenderer;
    private bool _hasStarted = false;

    private static readonly Color LockedColor = new(0.4f, 0.4f, 0.4f, 1f);

    public void SetLine(MachineLine line) => Line = line;

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        if (_spriteRenderer != null)
            _spriteRenderer.color = locked ? LockedColor : Color.white;

        if (!locked && _hasStarted)
        {
            NPCManagementSystem.Instance.RegisterMachine(this);
            NPCManagementSystem.Instance.TryFillMachineSlots(this);
        }
    }

    public Vector2 GetStandPosition() 
    {
        Vector2 pos;
        if (_standPoint != null)
        {
            pos = _standPoint.position;            
        } 
        else
        {
            pos = (Vector2)transform.position + Vector2.down * 0.6f;
        }
        pos.x = pos.x + 1 * Random.Range(-1.0f, 1.0f);
        return pos;
    }

    public bool TryOccupy(NPC npc)
    {
        if (!HasFreeSlot) return false;
        _occupants.Add(npc);
        return true;
    }

    public void FreeSlot(NPC npc)
    {
        _occupants.Remove(npc);
        NPCManagementSystem.Instance.OnMachineSlotFreed(this);
    }

    public float GetFairPayoutRate()
    {
        float rate = data.payoutRate;
        // if (Line != null) rate = Mathf.Clamp01(rate + Line.BonusPayoutRate); // TOFIX: see MachineLine.BonusPayoutRate
        return rate;
    }

    public (bool win, float amount) Play(float betAmount)
    {
        float payoutRate = (Line != null && Line.UsePayoutOverride) ? Line.PayoutOverride : data.payoutRate;
        // if (Line != null) payoutRate = Mathf.Clamp01(payoutRate + Line.BonusPayoutRate); // TOFIX: see MachineLine.BonusPayoutRate
        bool win = Random.value < payoutRate;
        float amount = win ? betAmount * data.winMultiplier : betAmount;

        float casinoNet = win ? betAmount * (1f - data.winMultiplier) : betAmount;
        casinoNet *= GameModifiers.revenueMultiplier;
        _totalRevenue += casinoNet;

        return (win, amount);
    }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _hasStarted = true;
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        _infoDisplay = GetComponentInChildren<MachineInfoDisplay>(true);

        if (!IsLocked)
            NPCManagementSystem.Instance.RegisterMachine(this);
    }

    void Update()
    {
        _trackingTime += Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_infoDisplay != null)
            _infoDisplay.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_infoDisplay != null)
            _infoDisplay.Hide();
    }
}
