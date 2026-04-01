using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    [Header("Data")]
    public MachineData data;

    [Header("Debug")]
    public float debugInterval = 3f;
    private float _debugTimer;

    public bool HasFreeSlot => _occupants.Count < data.totalSlots;

    private readonly List<NPC> _occupants = new();

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

    // Retourne si le NPC a gagné et le montant concerné (mise * winMultiplier si victoire, mise si défaite)
    public (bool win, float amount) Play(float betAmount)
    {
        bool win = Random.value < data.payoutRate;
        float amount = win ? betAmount * data.winMultiplier : betAmount;
        return (win, amount);
    }

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
        Debug.Log($"Machine {name} Occupants ({_occupants.Count}/{data.totalSlots}) : {names}");
    }
}
