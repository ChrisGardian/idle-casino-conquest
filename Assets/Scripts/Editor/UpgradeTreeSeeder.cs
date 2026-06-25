using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class UpgradeTreeSeeder
{
    private struct EffectSeed
    {
        public EffectType Type;
        public float Value;
        public string MachineLine;
    }

    private struct NodeSeed
    {
        public string Id;
        public string DisplayName;
        public string Description;
        public NodeType NodeType;
        public int MaxLevel;
        public float BaseCost;
        public float GrowthFactor;
        public string PrerequisiteId;
        public float PosX;
        public float PosY;
        public EffectSeed[] Effects;

        public NodeSeed(string id, string displayName, string description,
            NodeType nodeType, int maxLevel, float baseCost, float growthFactor,
            string prerequisiteId, float posX, float posY, EffectSeed[] effects = null)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            NodeType = nodeType;
            MaxLevel = maxLevel;
            BaseCost = baseCost;
            GrowthFactor = growthFactor;
            PrerequisiteId = prerequisiteId;
            PosX = posX;
            PosY = posY;
            Effects = effects ?? System.Array.Empty<EffectSeed>();
        }
    }

    private const float PositionScale = 0.75f;
    private static readonly Vector2 PositionOffset = new(-250f, -500f);

    // Layout: Casino <- (left) | Machines ^ (up) | NPCs -> (right) | VIPs --> (far right)
    private static readonly List<NodeSeed> Seeds = new()
    {
        new("open_casino",          "Open Casino",                  "Open your doors to the public.",
            NodeType.Unlock,  1,      0,   1.0f, "",                       0,      0,
            new[]{ E(EffectType.UnlockCasinoDoor, 1f) }),

        // General Casino (LEFT)
        new("global_revenue",       "Global Revenue Multiplier",    "Increases all casino earnings.",
            NodeType.Upgrade, 5,     50,  1.8f, "open_casino",          -600,   300,
            new[]{ E(EffectType.GlobalRevenueMultiplier, 0.1f) }),

        new("casino_reputation",    "Casino Reputation",            "Passively boosts popularity, increasing NPC spawn rate.",
            NodeType.Upgrade, 5,     40,  1.7f, "open_casino",          -600,  -300,
            new[]{ E(EffectType.CasinoReputationMultiplier, 0.05f) }),

        new("analytics_board",      "Analytics Board",              "Unlocks the detailed stats screen.",
            NodeType.Unlock,  1,    100,  1.0f, "open_casino",         -1200,     0,
            new[]{ E(EffectType.UnlockAnalyticsBoard, 1f) }),

        new("casino_expansion",     "Casino Expansion",             "Unlocks the second casino.",
            NodeType.Unlock,  1,   5000,  1.0f, "global_revenue",      -1800,   300,
            new[]{ E(EffectType.UnlockCasinoExpansion, 1f) }),

        // Machine Lines (UP)
        new("unlock_slots",         "Unlock Slots",                 "Unlock the slot machine line.",
            NodeType.Unlock,  1,     10,  1.0f, "open_casino",            0,   500,
            new[]{ E(EffectType.UnlockMachineLine, 1f, "SlotMachineLineData"), E(EffectType.UnlockNPCSpawning, 1f) }),

        new("unlock_roulette",      "Unlock Roulette",              "Unlock the roulette line.",
            NodeType.Unlock,  1,    200,  1.0f, "unlock_slots",           0,  1000,
            new[]{ E(EffectType.UnlockMachineLine, 1f, "RouletteLineData") }),

        new("unlock_blackjack",     "Unlock Blackjack",             "Unlock the blackjack line.",
            NodeType.Unlock,  1,    800,  1.0f, "unlock_roulette",        0,  1500,
            new[]{ E(EffectType.UnlockMachineLine, 1f, "BlackjackLineData") }),

        new("unlock_craps",         "Unlock Craps",                 "Unlock the craps line.",
            NodeType.Unlock,  1,   3000,  1.0f, "unlock_blackjack",      0,  2000,
            new[]{ E(EffectType.UnlockMachineLine, 1f, "CrapsLineData") }),

        new("unlock_bigsixwheel",   "Unlock Big Six Wheel",         "Unlock the big six wheel line.",
            NodeType.Unlock,  1,  10000,  1.0f, "unlock_craps",           0,  2500,
            new[]{ E(EffectType.UnlockMachineLine, 1f, "BigSixWheelLineData") }),

        // new("ml_payout_rate", "Payout Rate", ...) // TOFIX: see MachineLine.BonusPayoutRate

        new("ml_session_duration",  "Session Duration",             "Extends machine session duration.",
            NodeType.Upgrade, 5,     40,  1.7f, "unlock_slots",         600,  1100,
            new[]{ E(EffectType.MachineSessionDuration, 2f) }),

        // Add Machine per line
        new("add_slots_machine",        "Add Slot Machine",         "Add a machine to the Slots line.",
            NodeType.Upgrade, 5,    100,  2.0f, "unlock_slots",        -600,   500,
            new[]{ E(EffectType.MachineLineAddMachines, 1f, "SlotMachineLineData") }),

        new("add_roulette_machine",     "Add Roulette Table",       "Add a machine to the Roulette line.",
            NodeType.Upgrade, 5,    350,  2.0f, "unlock_roulette",     -600,  1000,
            new[]{ E(EffectType.MachineLineAddMachines, 1f, "RouletteLineData") }),

        new("add_blackjack_machine",    "Add Blackjack Table",      "Add a machine to the Blackjack line.",
            NodeType.Upgrade, 5,   1200,  2.0f, "unlock_blackjack",   -600,  1500,
            new[]{ E(EffectType.MachineLineAddMachines, 1f, "BlackjackLineData") }),

        new("add_craps_machine",        "Add Craps Table",          "Add a machine to the Craps line.",
            NodeType.Upgrade, 5,   4500,  2.0f, "unlock_craps",       -600,  2000,
            new[]{ E(EffectType.MachineLineAddMachines, 1f, "CrapsLineData") }),

        new("add_bigsixwheel_machine",  "Add Big Six Wheel",        "Add a machine to the Big Six Wheel line.",
            NodeType.Upgrade, 5,  15000,  2.0f, "unlock_bigsixwheel", -600,  2500,
            new[]{ E(EffectType.MachineLineAddMachines, 1f, "BigSixWheelLineData") }),

        new("add_common_machine",       "Common Machine",           "Add one machine to every unlocked line. Costs more.",
            NodeType.Upgrade, 3,  40000,  2.3f, "unlock_bigsixwheel", -1200, 2500,
            new[]{ E(EffectType.MachineLineAddMachinesAll, 1f) }),

        new("unlock_ml_info_panel", "Machine Line Info Panel",      "Unlocks the info panel for each machine line.",
            NodeType.Unlock,  1,    120,  1.0f, "unlock_slots",         600,  1600,
            new[]{ E(EffectType.UnlockMachineLineInfoPanel, 1f) }),

        new("unlock_m_info_panel",  "Machine Info Panel",           "Unlocks the info panel for individual machines.",
            NodeType.Unlock,  1,    250,  1.0f, "unlock_ml_info_panel",1200,  1600,
            new[]{ E(EffectType.UnlockMachineInfoPanel, 1f) }),

        new("unlock_payout_edit",   "Edit Payout Rate",             "Unlocks manual payout rate editing per line.",
            NodeType.Unlock,  1,    800,  1.0f, "unlock_ml_info_panel",1200,  2100,
            new[]{ E(EffectType.UnlockMachineLinePayoutEdit, 1f) }),

        // NPCs (RIGHT)
        new("npc_walk_speed",       "NPC Walk Speed",               "NPCs walk faster to machines.",
            NodeType.Upgrade, 5,     20,  1.6f, "open_casino",          800,   200,
            new[]{ E(EffectType.NPCWalkSpeed, 0.1f) }),

        new("npc_arrival_interval", "NPC Arrival Interval",         "Reduces time between NPC spawns.",
            NodeType.Upgrade, 5,     50,  1.8f, "open_casino",          800,     0,
            new[]{ E(EffectType.NPCArrivalInterval, 0.4f) }),

        new("npc_patience",         "NPC Patience",                 "NPCs wait longer before leaving.",
            NodeType.Upgrade, 5,     15,  1.7f, "open_casino",          800,  -200,
            new[]{ E(EffectType.NPCPatience, 2f) }),

        new("npc_colors",           "NPC Colors",                   "Unlocks color variants for NPCs.",
            NodeType.Unlock,  1,     25,  1.0f, "open_casino",          800,   400,
            new[]{ E(EffectType.UnlockNPCColors, 1f) }),

        new("npc_base_gains",       "NPC Base Gains Multiplier",    "Multiplies earnings from regular NPCs.",
            NodeType.Upgrade, 5,     30,  1.9f, "open_casino",          800,  -400,
            new[]{ E(EffectType.NPCBaseGainsMultiplier, 0.1f) }),

        new("npc_satisfaction",     "NPC Satisfaction Threshold",   "NPCs contribute more to your reputation on departure, even when they lose often.",
            NodeType.Upgrade, 5,    100,  1.7f, "npc_patience",        1500,  -200,
            new[]{ E(EffectType.NPCSatisfactionThreshold, 0.05f) }),

        new("npc_departure_gain",   "Departure Patience Bonus",     "NPCs gain more patience per win, letting them stay and play longer.",
            NodeType.Upgrade, 5,    300,  1.8f, "npc_satisfaction",    2200,  -200,
            new[]{ E(EffectType.NPCDepartureSatisfactionGain, 0.5f) }),

        // VIPs (FAR RIGHT)
        new("unlock_vips",          "Unlock VIPs",                  "Enables VIP NPCs to spawn.",
            NodeType.Unlock,  1,    500,  1.0f, "npc_arrival_interval",2200,     0,
            new[]{ E(EffectType.UnlockVIPs, 1f) }),

        new("vip_spawn_chance",     "VIP Spawn Chance",             "Increases the chance of a VIP spawning.",
            NodeType.Upgrade, 5,    300,  1.9f, "unlock_vips",         2800,   200,
            new[]{ E(EffectType.VIPSpawnChance, 0.02f) }),

        new("vip_walk_speed",       "VIP Walk Speed",               "VIPs walk faster.",
            NodeType.Upgrade, 5,    200,  1.6f, "unlock_vips",         2800,  -300,
            new[]{ E(EffectType.VIPWalkSpeed, 0.1f) }),

        new("vip_patience",         "VIP Patience",                 "VIPs stay longer at machines.",
            NodeType.Upgrade, 5,    250,  1.7f, "unlock_vips",         2800,     0,
            new[]{ E(EffectType.VIPPatience, 3f) }),

        new("vip_base_gains",       "VIP Base Gains Multiplier",    "Multiplies earnings from VIPs.",
            NodeType.Upgrade, 5,    400,  2.0f, "unlock_vips",         2800,   500,
            new[]{ E(EffectType.VIPBaseGainsMultiplier, 0.15f) }),

        // new("unlock_vip_referral",  "VIP Referral",                 "A satisfied VIP temporarily boosts the next VIP spawn chance.",
        //     NodeType.Unlock,  1,   1500,  1.0f, "vip_spawn_chance",    3400,   200,
        //     new[]{ E(EffectType.UnlockVIPReferral, 1f) }),

        // new("vip_referral_chance",  "VIP Referral Chance",          "Increases the strength of the VIP referral bonus.",
        //     NodeType.Upgrade, 5,   1000,  1.9f, "unlock_vip_referral", 3400,   500,
        //     new[]{ E(EffectType.VIPReferralChance, 0.05f) }),

        // new("unlock_golden_vips",   "Unlock Golden VIPs",           "Enables Golden VIP NPCs to spawn.",
        //     NodeType.Unlock,  1,   5000,  1.0f, "vip_base_gains",      3400,   700,
        //     new[]{ E(EffectType.UnlockGoldenVIPs, 1f) }),
    };

    [MenuItem("Casino/Generate Upgrade Tree")]
    public static void Generate()
    {
        string path = EditorUtility.OpenFilePanel("Select UpgradeTreeData asset", "Assets/Data", "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = "Assets" + path.Substring(Application.dataPath.Length);
        UpgradeTreeData treeData = AssetDatabase.LoadAssetAtPath<UpgradeTreeData>(path);
        if (treeData == null)
        {
            Debug.LogError("UpgradeTreeSeeder: selected file is not an UpgradeTreeData asset.");
            return;
        }

        treeData.nodes.Clear();

        foreach (NodeSeed seed in Seeds)
        {
            List<UpgradeEffect> effects = new();
            foreach (EffectSeed es in seed.Effects)
            {
                MachineLineData lineData = null;
                if (!string.IsNullOrEmpty(es.MachineLine))
                {
                    string linePath = $"Assets/Data/MachineLines/{es.MachineLine}.asset";
                    lineData = AssetDatabase.LoadAssetAtPath<MachineLineData>(linePath);
                    if (lineData == null)
                        Debug.LogWarning($"UpgradeTreeSeeder: MachineLineData not found at {linePath}");
                }
                effects.Add(new UpgradeEffect { type = es.Type, valuePerLevel = es.Value, targetLine = lineData });
            }

            UpgradeNodeDefinition node = new()
            {
                id             = seed.Id,
                displayName    = seed.DisplayName,
                description    = seed.Description,
                nodeType       = seed.NodeType,
                maxLevel       = seed.MaxLevel,
                baseCost       = seed.BaseCost,
                growthFactor   = seed.GrowthFactor,
                prerequisiteId = seed.PrerequisiteId,
                treePosition   = new Vector2(seed.PosX, seed.PosY) * PositionScale + PositionOffset,
                effects        = effects,
            };

            treeData.nodes.Add(node);
        }

        EditorUtility.SetDirty(treeData);
        AssetDatabase.SaveAssets();

        Debug.Log($"UpgradeTreeSeeder: generated {treeData.nodes.Count} nodes into {path}");
    }

    private static EffectSeed E(EffectType type, float value, string machineLine = null) =>
        new() { Type = type, Value = value, MachineLine = machineLine };
}