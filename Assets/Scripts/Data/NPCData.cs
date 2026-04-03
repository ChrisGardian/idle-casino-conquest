using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Casino/NPCData")]
public class NPCData : ScriptableObject
{
    [Header("Identity")]
    public bool isVIP;

    [Header("Patience")]
    [Tooltip("Durée de patience totale du NPC en secondes")]
    public float patienceTotal = 60f;

    [Header("Economy")]
    [Tooltip("Mise de base du NPC")]
    public float baseBetAmount = 10f;

    [Header("Gameplay")]
    [Tooltip("Gain de patience par victoire (avant multiplicateur machine)")]
    public float basePatienceWinGain = 0.5f;

    [Tooltip("Intervalle entre chaque mise en secondes")]
    public float playInterval = 2f;

    [Header("Movement")]
    [Tooltip("Vitesse de déplacement (marche vers machine et sortie)")]
    public float walkSpeed = 2f;

    [Tooltip("Rayon de divagation autour du point de spawn")]
    public float wanderRadius = 3f;

    [Tooltip("Pause minimale entre deux déplacements aléatoires (secondes)")]
    public float wanderPauseMin = 1f;

    [Tooltip("Pause maximale entre deux déplacements aléatoires (secondes)")]
    public float wanderPauseMax = 3f;
}
