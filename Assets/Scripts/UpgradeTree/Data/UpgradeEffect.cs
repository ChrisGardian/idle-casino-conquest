using UnityEngine;

public enum EffectType
{
    // Unlocks structurels
    UnlockNPCSpawning,
    UnlockMachineLine,
    UnlockVIPs,
    UnlockGoldenVIPs,
    UnlockNPCColors,
    UnlockMachineLineInfoPanel,
    UnlockMachineInfoPanel,
    UnlockMachineLinePayoutEdit,
    UnlockVIPReferral,
    UnlockAnalyticsBoard,
    UnlockCasinoExpansion,
    UnlockCasinoDoor,

    // Machine Lines
    MachineLineAddMachines,
    MachineLineAddMachinesAll,
    MachineLineSlotsPerMachine,
    // MachinePayoutRate, // TOFIX: see MachineLine.BonusPayoutRate
    MachineSessionDuration,

    // NPCs
    NPCArrivalInterval,
    NPCWalkSpeed,
    NPCPatience,
    NPCBaseGainsMultiplier,
    NPCSatisfactionThreshold,
    NPCDepartureSatisfactionGain,

    // VIPs
    VIPSpawnChance,
    VIPWalkSpeed,
    VIPPatience,
    VIPBaseGainsMultiplier,
    VIPReferralChance,

    // Global
    GlobalRevenueMultiplier,
    CasinoReputationMultiplier,
}

[System.Serializable]
public class UpgradeEffect
{
    public EffectType type;
    public float valuePerLevel;
    public MachineLineData targetLine;
}
