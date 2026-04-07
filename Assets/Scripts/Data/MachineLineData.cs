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
    [Tooltip("Coût de chaque niveau d'upgrade (index 0 = passage au niveau 1, etc.)")]
    public float[] upgradeCosts;

    [Tooltip("Nombre max de machines autorisées par niveau (index 0 = niveau 0, etc.)")]
    public int[] maxMachinesPerLevel;

    [Tooltip("Bonus de revenue appliqué à toutes les machines de la ligne par niveau")]
    public float[] revenueMultiplierPerLevel;
}
