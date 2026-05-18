using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTreeManager : MonoBehaviour
{
    public static UpgradeTreeManager Instance { get; private set; }

    [SerializeField] private UpgradeTreeData _data;

    private readonly Dictionary<string, int> _nodeLevels = new();

    public int AvailableActionsCount { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RecalculateAvailableCount();
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    public int GetLevel(string nodeId) =>
        _nodeLevels.TryGetValue(nodeId, out int level) ? level : 0;

    public UpgradeNodeDefinition GetNodeDefinition(string nodeId) =>
        _data.GetNode(nodeId);

    public bool IsUnlocked(string nodeId) => GetLevel(nodeId) > 0;

    public float GetCost(UpgradeNodeDefinition node)
    {
        int level = GetLevel(node.id);
        float cost = node.baseCost * Mathf.Pow(node.growthFactor, level);
        return Mathf.Round(cost * _data.globalCostMultiplier);
    }

    public bool IsAccessible(string nodeId)
    {
        UpgradeNodeDefinition node = _data.GetNode(nodeId);
        if (node == null) return false;
        if (string.IsNullOrEmpty(node.prerequisiteId)) return true;
        return IsUnlocked(node.prerequisiteId);
    }

    public bool CanAct(string nodeId)
    {
        if (!IsAccessible(nodeId)) return false;
        UpgradeNodeDefinition node = _data.GetNode(nodeId);
        if (node == null) return false;
        if (GetLevel(nodeId) >= node.maxLevel) return false;
        return CurrencyManager.Instance.money >= GetCost(node);
    }

    // ── Action ────────────────────────────────────────────────────────────────

    public bool TryAct(string nodeId)
    {
        if (!CanAct(nodeId)) return false;
        UpgradeNodeDefinition node = _data.GetNode(nodeId);
        if (!CurrencyManager.Instance.TrySpendMoney(GetCost(node))) return false;

        _nodeLevels[nodeId] = GetLevel(nodeId) + 1;
        int newLevel = _nodeLevels[nodeId];

        foreach (UpgradeEffect effect in node.effects)
            ApplyEffect(effect, newLevel);

        RecalculateAvailableCount();
        return true;
    }

    // ── Effects ───────────────────────────────────────────────────────────────

    private void ApplyEffect(UpgradeEffect effect, int newLevel)
    {
        MachineLine line = effect.targetLine != null
            ? MachineLineManager.Instance?.FindLine(effect.targetLine)
            : null;

        switch (effect.type)
        {
            case EffectType.UnlockMachineLine:
                Debug.Log($"Unlocked Machine Line {effect.targetLine?.name}");
                line?.Unlock();
                break;

            case EffectType.MachineLineAddMachines:
                line?.AddMachinesFromUpgrade((int)effect.valuePerLevel);
                break;

            case EffectType.MachinePayoutRate:
                line?.AddBonusPayoutRate(effect.valuePerLevel);
                break;

            case EffectType.GlobalRevenueMultiplier:
                CurrencyManager.Instance?.AddRevenueMultiplier(effect.valuePerLevel);
                break;

            case EffectType.UnlockNPCSpawning:
                NPCSpawner.Instance?.Activate();
                break;

            case EffectType.UnlockVIPs:
                NPCSpawner.Instance?.UnlockVIPs();
                break;

            case EffectType.UnlockMachineLineInfoPanel:
                MachineLineInfoPopup.Unlock();
                break;

            case EffectType.UnlockMachineInfoPanel:
                MachineInfoDisplay.Unlock();
                break;

            case EffectType.UnlockMachineLinePayoutEdit:
                MachineLineEditPopup.Unlock();
                break;

            case EffectType.NPCArrivalInterval:
                NPCSpawner.Instance?.AddArrivalIntervalBonus(effect.valuePerLevel);
                break;

            case EffectType.VIPSpawnChance:
                NPCSpawner.Instance?.AddVIPSpawnChanceBonus(effect.valuePerLevel);
                break;

            case EffectType.MachineSessionDuration:
                line?.AddBonusSessionDuration(effect.valuePerLevel);
                break;

            case EffectType.CasinoReputationMultiplier:
                CurrencyManager.Instance?.AddReputationMultiplier(effect.valuePerLevel);
                break;

            case EffectType.UnlockCasinoDoor:
                // TODO: ouvrir la sprite de la porte du casino
                break;

            default:
                break;
        }
    }

    // ── Badge ─────────────────────────────────────────────────────────────────

    private void RecalculateAvailableCount()
    {
        int count = 0;
        foreach (UpgradeNodeDefinition node in _data.nodes)
        {
            if (CanAct(node.id)) count++;
        }
        AvailableActionsCount = count;
    }
}
