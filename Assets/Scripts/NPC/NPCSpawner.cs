using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Zone")]
    public Vector2 spawnZoneSize = new Vector2(5f, 3f);

    public GameObject npcPrefab;
    public GameObject npcParent;
    public float baseSpawnInterval = 10f;

    private float _spawnTimer;
    private int _npcCount = 0;

    void Start() => _spawnTimer = GetSpawnInterval();

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnNPC();
            _spawnTimer = GetSpawnInterval();
        }
    }

    private void SpawnNPC()
    {
        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-spawnZoneSize.x / 2f, spawnZoneSize.x / 2f),
            Random.Range(-spawnZoneSize.y / 2f, spawnZoneSize.y / 2f),
            0f
            );
        GameObject obj = Instantiate(npcPrefab, spawnPos, Quaternion.identity, npcParent.transform);
        obj.name = $"NPC_{_npcCount++}";
        NPC npc = obj.GetComponent<NPC>();
        NPCManagementSystem.Instance.RegisterNPC(npc);
    }

    private float GetSpawnInterval()
    {
        return baseSpawnInterval / CurrencyManager.Instance.popularity;
    }
}