using UnityEngine;

[CreateAssetMenu(fileName = "MachineLineData", menuName = "Casino/MachineLineData")]
public class MachineLineData : ScriptableObject
{
    [Header("Identity")]
    public string lineTypeName;
    public GameObject machinePrefab;

    [Header("Unlock")]
    public float unlockCost;

    [Header("Upgrades")]
    [Tooltip("Cost of each upgrade level (index 0 = upgrade to level 1, etc.)")]
    public float[] upgradeCosts;

    [Tooltip("Max machines allowed per level (index 0 = level 0, etc.)")]
    public int[] maxMachinesPerLevel;

    [Tooltip("Revenue bonus applied to all machines on this line per level")]
    public float[] revenueMultiplierPerLevel;
}