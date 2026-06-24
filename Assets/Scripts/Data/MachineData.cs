using UnityEngine;

[CreateAssetMenu(fileName = "MachineData", menuName = "Casino/MachineData")]
public class MachineData : ScriptableObject
{
    [Header("Slots")]
    public int totalSlots = 1;

    [Header("Economy")]
    [Tooltip("Probability that the NPC wins (0 to 1)")]
    public float payoutRate = 0.35f;

    [Tooltip("Multiplier applied to the bet on a win")]
    public float winMultiplier = 2f;

    [Tooltip("Multiplier applied to the NPC's bet amount")]
    public float betAmountMultiplier = 1f;

    [Header("Session")]
    [Tooltip("Session duration in seconds")]
    public float sessionDuration = 15f;

    [Tooltip("Patience gain multiplier on a win")]
    public float patienceWinGainMultiplier = 1f;
}