using System.Collections.Generic;
using UnityEngine;

public class NPCManagementSystem : MonoBehaviour
{
    [Header("Variables")]
    public float popularityModifier = 1f;

    [Header("Wander Bounds")]
    [SerializeField] private BoxCollider2D _wanderBoundsCollider;

    public static NPCManagementSystem Instance { get; private set; }
    public static Bounds WanderBounds { get; private set; }

    private readonly List<NPC> _allNPCs = new();
    private readonly List<NPC> _idleNPCs = new();
    private readonly List<Machine> _machines = new();

    public float numberOfNPCs => _allNPCs.Count;

    public int TotalSlots
    {
        get { int n = 0; foreach (var m in _machines) n += m.data.totalSlots; return n; }
    }

    public int TotalFreeSlots
    {
        get { int n = 0; foreach (var m in _machines) n += m.data.totalSlots - m.Occupants.Count; return n; }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (_wanderBoundsCollider != null)
            WanderBounds = _wanderBoundsCollider.bounds;
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
        float satisfaction = Mathf.Clamp01(npc.ComputeSatisfaction() + GameModifiers.npcSatisfactionThresholdBonus);
        float currentPopularity = CurrencyManager.Instance.popularity;
        float popularityDelta = currentPopularity * Mathf.Lerp(-popularityModifier, popularityModifier, satisfaction);
        CurrencyManager.Instance.AddPopularity(popularityDelta);

        _allNPCs.Remove(npc);
        _idleNPCs.Remove(npc);
        Destroy(npc.gameObject);
    }

    private void TryAssignNPC(NPC npc)
    {
        var shuffled = new List<Machine>(_machines);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (var machine in shuffled)
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