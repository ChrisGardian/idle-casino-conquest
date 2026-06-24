using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [SerializeField] private FloatingText _prefab;

    [Tooltip("Vertical offset above the spawn point")]
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

    public void Spawn(Vector3 worldPos, float amount)
    {
        if (_prefab == null)
        {
            Debug.LogWarning("[FloatingTextSpawner] Prefab not assigned.");
            return;
        }

        float jitter = Random.Range(-0.25f, 0.25f);
        Vector3 spawnPos = worldPos + new Vector3(jitter, _yOffset, -0.1f);

        var ft = Instantiate(_prefab, spawnPos, Quaternion.identity);
        ft.Setup(amount);
    }
}