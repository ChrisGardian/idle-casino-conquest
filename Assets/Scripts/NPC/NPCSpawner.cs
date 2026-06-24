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
    [Tooltip("Popularity exponent (0.5 = square root, softens the effect)")]
    [Range(0.1f, 1f)]
    public float popularityExponent = 0.5f;
    [Tooltip("Spawn interval multiplier when all slots are occupied (> 1 = slower)")]
    public float crowdedIntervalMultiplier = 1.5f;
    [Tooltip("Spawn interval multiplier when all slots are free (< 1 = faster)")]
    public float spaciousIntervalMultiplier = 0.75f;
    [Tooltip("Minimum guaranteed interval between spawns (prevents infinite loop)")]
    public float minSpawnInterval = 0.5f;

    [Range(0f, 1f)]
    public float vipSpawnChance = 0.1f;

    [Header("NPC Colors")]
    [Range(0f, 1f)] public float tintSaturationMin = 0.25f;
    [Range(0f, 1f)] public float tintSaturationMax = 0.55f;
    [Range(0f, 1f)] public float tintValueMin = 0.75f;
    [Range(0f, 1f)] public float tintValueMax = 1f;

    public static NPCSpawner Instance { get; private set; }

    private float _spawnTimer;
    private int _npcCount = 0;
    private bool _active = false;
    private bool _vipsUnlocked = false;
    private bool _npcColorsUnlocked = false;

    public void UnlockVIPs() => _vipsUnlocked = true;
    public void UnlockNPCColors() => _npcColorsUnlocked = true;

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
        npc.SetTint(GetRandomTint());
        NPCManagementSystem.Instance.RegisterNPC(npc);
    }

    private Color GetRandomTint()
    {
        if (!_npcColorsUnlocked) return Color.white;
        float h = Random.value;
        float s = Random.Range(tintSaturationMin, tintSaturationMax);
        float v = Random.Range(tintValueMin, tintValueMax);
        return Color.HSVToRGB(h, s, v);
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

        return Mathf.Max(minSpawnInterval, interval);
    }
}