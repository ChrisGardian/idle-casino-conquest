using UnityEngine;

[CreateAssetMenu(fileName = "MachineData", menuName = "Casino/MachineData")]
public class MachineData : ScriptableObject
{
    [Header("Slots")]
    public int totalSlots = 1;

    [Header("Economy")]
    [Tooltip("Probabilité que le NPC gagne (0 à 1)")]
    public float payoutRate = 0.35f;

    [Tooltip("Multiplicateur appliqué à la mise en cas de victoire")]
    public float winMultiplier = 2f;

    [Tooltip("Multiplicateur de la mise du NPC")]
    public float betAmountMultiplier = 1f;

    [Header("Session")]
    [Tooltip("Durée de la session en secondes")]
    public float sessionDuration = 15f;

    [Tooltip("Multiplicateur du gain de patience en cas de victoire")]
    public float patienceWinGainMultiplier = 1f;
}
