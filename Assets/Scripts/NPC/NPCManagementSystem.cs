using System.Collections.Generic;
using UnityEngine;

public class NPCManagementSystem : MonoBehaviour
{
    [Header("Variables")]
    public float popularityModifier = 1f;

    public static NPCManagementSystem Instance { get; private set; }

    private readonly List<NPC> _allNPCs = new();
    private readonly List<NPC> _idleNPCs = new();
    private readonly List<Machine> _machines = new();

    public float numberOfNPCs => _allNPCs.Count; 

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterMachine(Machine machine) => _machines.Add(machine);

    // Appelé par le Spawner
    public void RegisterNPC(NPC npc)
    {
        _allNPCs.Add(npc);
        _idleNPCs.Add(npc);
        TryAssignNPC(npc);
    }

    // Appelé par Machine.FreeSlot()
    public void OnMachineSlotFreed(Machine machine)
    {
        if (_idleNPCs.Count == 0) return;
        NPC npc = _idleNPCs[0];
        TryAssignToMachine(npc, machine);
    }

    // Appelé par NPC quand il redevient Idle après une session
    public void OnNPCBecameIdle(NPC npc)
    {
        _idleNPCs.Add(npc);
        TryAssignNPC(npc);
    }

    // Appelé quand patienceTotal = 0 et machine libérée (ou pas de machine)
    public void OnNPCLeaving(NPC npc)
    {
        float satisfaction = npc.ComputeSatisfaction();
        // satisfaction = 0 si n'a jamais joué → malus max
        float popularityDelta = Mathf.Lerp(-popularityModifier, popularityModifier, satisfaction);
        CurrencyManager.Instance.AddPopularity(popularityDelta);

        _allNPCs.Remove(npc);
        _idleNPCs.Remove(npc);
        Destroy(npc.gameObject);
    }

    private void TryAssignNPC(NPC npc)
    {
        foreach (var machine in _machines)
        {
            if (machine.HasFreeSlot)
            {
                TryAssignToMachine(npc, machine);
                return;
            }
        }
        // Aucune machine dispo → NPC reste Idle, partira avec satisfaction = 0
    }

    private void TryAssignToMachine(NPC npc, Machine machine)
    {
        if (!machine.TryOccupy(npc)) return;
        _idleNPCs.Remove(npc);
        npc.AssignMachine(machine);
    }
}