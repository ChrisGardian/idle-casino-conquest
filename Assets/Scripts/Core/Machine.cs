using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    [Header("Settings")]
    public int totalSlots = 1;

    [Header("Debug")]
    public float debugInterval = 3f;
    private float _debugTimer;

    public virtual float payoutRate => _payoutRate;
    public virtual float sessionDuration => _sessionDuration;
    public virtual float patienceWinGainMultiplier => _patienceWinGainMultiplier;
    public virtual float betAmountMultiplier => _betAmountMultiplier;

    [SerializeField] private float _payoutRate = 0.35f;
    [SerializeField] private float _sessionDuration = 15f;
    [SerializeField] private float _patienceWinGainMultiplier = 1f;
    [SerializeField] private float _betAmountMultiplier = 1f;

    private readonly List<NPC> _occupants = new();

    public bool HasFreeSlot => _occupants.Count < totalSlots;

    public bool TryOccupy(NPC npc)
    {
        if (!HasFreeSlot) return false;
        _occupants.Add(npc);
        return true;
    }

    public void FreeSlot(NPC npc)
    {
        _occupants.Remove(npc);
        Debug.Log($"{npc.name} just stopped playing");
        NPCManagementSystem.Instance.OnMachineSlotFreed(this);
    }

    public bool Play() => Random.value < payoutRate;

    void Start()
    {
        NPCManagementSystem.Instance.RegisterMachine(this);
    }

    void Update()
    {
        _debugTimer -= Time.deltaTime;
        if (_debugTimer <= 0f)
        {
            _debugTimer = debugInterval;
            PrintOccupants();
        }
    }
    private void PrintOccupants()
    {
        if (_occupants.Count == 0)
        {
            Debug.Log($"Machine {name} empty");
            return;
        }
        string names = string.Join(", ", _occupants.ConvertAll(n => n.name));
        Debug.Log($"Machine {name} Occupants ({_occupants.Count}/{totalSlots}) : {names}");
    }
}