using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTreeManager : MonoBehaviour
{
    public static UpgradeTreeManager Instance { get; private set; }

    [SerializeField] private UpgradeTreeData _data;

#if UNITY_EDITOR
    [SerializeField] private bool _debugFreeUpgrades = false;
#endif

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
#if UNITY_EDITOR
        if (_debugFreeUpgrades) return 0f;
#endif
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

            case EffectType.MachineLineAddMachinesAll:
                if (MachineLineManager.Instance != null)
                    foreach (MachineLine l in MachineLineManager.Instance.Lines)
                        if (l.IsUnlocked) l.AddMachinesFromUpgrade((int)effect.valuePerLevel);
                break;

            case EffectType.MachinePayoutRate:
                if (line != null)
                    line.AddBonusPayoutRate(effect.valuePerLevel);
                else if (MachineLineManager.Instance != null)
                    foreach (MachineLine l in MachineLineManager.Instance.Lines)
                        l.AddBonusPayoutRate(effect.valuePerLevel);
                break;

            case EffectType.GlobalRevenueMultiplier:
                GameModifiers.revenueMultiplier += effect.valuePerLevel;
                break;

            case EffectType.UnlockNPCSpawning:
                NPCSpawner.Instance?.Activate();
                break;

            case EffectType.UnlockNPCColors:
                NPCSpawner.Instance?.UnlockNPCColors();
                break;

            case EffectType.UnlockVIPs:
                NPCSpawner.Instance?.UnlockVIPs();
                break;

            case EffectType.NPCWalkSpeed:
                GameModifiers.npcWalkSpeedBonus += effect.valuePerLevel;
                break;

            case EffectType.NPCPatience:
                GameModifiers.npcPatienceBonus += effect.valuePerLevel;
                break;

            case EffectType.NPCBaseGainsMultiplier:
                GameModifiers.npcBetMultiplierBonus += effect.valuePerLevel;
                break;

            case EffectType.NPCSatisfactionThreshold:
                GameModifiers.npcSatisfactionThresholdBonus += effect.valuePerLevel;
                break;

            case EffectType.NPCDepartureSatisfactionGain:
                GameModifiers.npcPatienceWinGainBonus += effect.valuePerLevel;
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
                GameModifiers.npcArrivalIntervalBonus += effect.valuePerLevel;
                break;

            case EffectType.VIPSpawnChance:
                GameModifiers.vipSpawnChanceBonus += effect.valuePerLevel;
                break;

            case EffectType.MachineSessionDuration:
                if (line != null)
                    line.AddBonusSessionDuration(effect.valuePerLevel);
                else if (MachineLineManager.Instance != null)
                    foreach (MachineLine l in MachineLineManager.Instance.Lines)
                        l.AddBonusSessionDuration(effect.valuePerLevel);
                break;

            case EffectType.MachineLineSlotsPerMachine:
                line?.AddBonusSlotsPerMachine((int)effect.valuePerLevel);
                break;

            case EffectType.VIPWalkSpeed:
                GameModifiers.vipWalkSpeedBonus += effect.valuePerLevel;
                break;

            case EffectType.VIPPatience:
                GameModifiers.vipPatienceBonus += effect.valuePerLevel;
                break;

            case EffectType.VIPBaseGainsMultiplier:
                GameModifiers.vipBetMultiplierBonus += effect.valuePerLevel;
                break;

            case EffectType.UnlockAnalyticsBoard:
                // TODO: GameUI.Instance?.UnlockAnalyticsBoard();
                Debug.Log("UpgradeTreeManager: UnlockAnalyticsBoard – handler à brancher sur GameUI.");
                break;

            case EffectType.UnlockCasinoExpansion:
                // TODO: CasinoExpansionManager.Instance?.Unlock();
                Debug.Log("UpgradeTreeManager: UnlockCasinoExpansion – handler à brancher sur la scène 2.");
                break;

            // VIPReferralChance / UnlockVIPReferral : système de referral non implémenté not for the Early Access
            case EffectType.VIPReferralChance:
            case EffectType.UnlockVIPReferral:
                break;

            case EffectType.CasinoReputationMultiplier:
                GameModifiers.reputationMultiplier += effect.valuePerLevel;
                break;

            case EffectType.UnlockCasinoDoor:
                CasinoDoor.Instance?.Open();
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
