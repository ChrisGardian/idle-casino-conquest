using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Casino/NPCData")]
public class NPCData : ScriptableObject
{
    [Header("Identity")]
    public bool isVIP;

    [Header("Patience")]
    [Tooltip("Total patience duration in seconds")]
    public float patienceTotal = 60f;

    [Header("Economy")]
    [Tooltip("Base bet amount for this NPC")]
    public float baseBetAmount = 10f;

    [Header("Gameplay")]
    [Tooltip("Patience gained per win (before machine multiplier)")]
    public float basePatienceWinGain = 0.5f;

    [Tooltip("Interval between each bet in seconds")]
    public float playInterval = 2f;

    [Header("Movement")]
    [Tooltip("Walk speed (towards machine and towards exit)")]
    public float walkSpeed = 2f;

    [Tooltip("Wander radius around spawn point")]
    public float wanderRadius = 3f;

    [Tooltip("Minimum pause between two random moves (seconds)")]
    public float wanderPauseMin = 1f;

    [Tooltip("Maximum pause between two random moves (seconds)")]
    public float wanderPauseMax = 3f;
}