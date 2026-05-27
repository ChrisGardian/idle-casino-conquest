using UnityEngine;

/// <summary>
/// Singleton qui instancie des FloatingText dans le monde.
/// À placer sur un GameObject persistent de la scène (ex: GameManagers).
/// Assigner le prefab FloatingText dans l'inspecteur
/// (Casino > Créer Prefab FloatingText pour le générer automatiquement).
/// </summary>
public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [SerializeField] private FloatingText _prefab;

    [Tooltip("Décalage vertical au dessus du point de spawn")]
    [SerializeField] private float _yOffset = 0.8f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Affiche un bilan flottant à la position donnée.
    /// Vert si positif (casino gagne), rouge si négatif (casino perd).
    /// Un léger jitter horizontal évite l'empilement si plusieurs NPCs partent en même temps.
    /// </summary>
    public void Spawn(Vector3 worldPos, float amount)
    {
        if (_prefab == null)
        {
            Debug.LogWarning("[FloatingTextSpawner] Prefab non assigné ! Lance Casino > Créer Prefab FloatingText.");
            return;
        }

        float jitter = Random.Range(-0.25f, 0.25f);
        Vector3 spawnPos = worldPos + new Vector3(jitter, _yOffset, -0.1f);
        Debug.Log($"[FloatingText] Spawn {amount:+0.0;-0.0}$ à {spawnPos}");

        var ft = Instantiate(_prefab, spawnPos, Quaternion.identity);
        ft.Setup(amount);
    }
}
