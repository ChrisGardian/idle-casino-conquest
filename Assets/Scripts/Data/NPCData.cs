using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Casino/NPCData")]
public class NPCData : ScriptableObject
{
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
}
