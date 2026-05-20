using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Zone")]
    public Vector2 spawnZoneSize = new Vector2(5f, 3f);

    public GameObject npcPrefab;
    public GameObject vipPrefab;
    public GameObject npcParent;
    public float baseSpawnInterval = 40f;
    public float firstSpawnDelay = 3f;

    [Header("Spawn Tuning")]
    [Tooltip("Exposant de la popularity (0.5 = racine carrée, atténue l'effet)")]
    [Range(0.1f, 1f)]
    public float popularityExponent = 0.5f;
    [Tooltip("Multiplicateur d'intervalle quand toutes les places sont occupées (> 1 = plus lent)")]
    public float crowdedIntervalMultiplier = 1.5f;
    [Tooltip("Multiplicateur d'intervalle quand toutes les places sont libres (< 1 = plus rapide)")]
    public float spaciousIntervalMultiplier = 0.75f;

    [Range(0f, 1f)]
    public float vipSpawnChance = 0.1f;

    [Header("NPC Colors")]
    public Color[] npcColorPalette;
    public Color[] vipColorPalette;

    private static readonly Color[] _defaultNpcColors = {
        new Color(1f, 0.82f, 0.70f), // peau claire
        new Color(0.87f, 0.65f, 0.47f), // peau dorée
        new Color(0.60f, 0.40f, 0.27f), // peau foncée
        new Color(0.70f, 0.85f, 1f),  // bleuté
        new Color(0.85f, 1f, 0.75f),  // verdâtre
        new Color(1f, 0.78f, 0.85f),  // rosé
    };

    private static readonly Color[] _defaultVipColors = {
        new Color(1f, 0.88f, 0.40f),  // or
        new Color(0.90f, 0.90f, 0.95f), // argent
        new Color(0.95f, 0.80f, 0.60f), // champagne
    };

    public static NPCSpawner Instance { get; private set; }

    private float _spawnTimer;
    private int _npcCount = 0;
    private bool _active = false;
    private bool _vipsUnlocked = false;

    public void UnlockVIPs() => _vipsUnlocked = true;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Activate()
    {
        if (_active) return;
        _active = true;
        _spawnTimer = firstSpawnDelay;
    }

    void Update()
    {
        if (!_active) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnNPC();
            _spawnTimer = GetSpawnInterval();
        }
    }

    private void SpawnNPC()
    {
        Vector3 spawnPos = npcParent.transform.position + new Vector3(
            Random.Range(-spawnZoneSize.x / 2f, spawnZoneSize.x / 2f),
            Random.Range(-spawnZoneSize.y / 2f, spawnZoneSize.y / 2f),
            0f
            );

        bool spawnVIP = _vipsUnlocked && vipPrefab != null && Random.value < Mathf.Clamp01(vipSpawnChance + GameModifiers.vipSpawnChanceBonus);
        GameObject prefab = spawnVIP ? vipPrefab : npcPrefab;

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, npcParent.transform);
        obj.name = spawnVIP ? $"VIP_{_npcCount++}" : $"NPC_{_npcCount++}";
        NPC npc = obj.GetComponent<NPC>();
        Color[] palette = spawnVIP
            ? (vipColorPalette?.Length > 0 ? vipColorPalette : _defaultVipColors)
            : (npcColorPalette?.Length > 0 ? npcColorPalette : _defaultNpcColors);
        npc.SetTint(palette[Random.Range(0, palette.Length)]);
        NPCManagementSystem.Instance.RegisterNPC(npc);
    }

    private float GetSpawnInterval()
    {
        float interval = baseSpawnInterval / (1f + GameModifiers.npcArrivalIntervalBonus) / Mathf.Pow(CurrencyManager.Instance.popularity, popularityExponent);

        int totalSlots = NPCManagementSystem.Instance.TotalSlots;
        if (totalSlots > 0)
        {
            float freeRatio = (float)NPCManagementSystem.Instance.TotalFreeSlots / totalSlots;
            interval *= Mathf.Lerp(crowdedIntervalMultiplier, spaciousIntervalMultiplier, freeRatio);
        }

        return interval;
    }

}