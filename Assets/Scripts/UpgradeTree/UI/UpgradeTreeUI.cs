using System.Collections.Generic;
using UnityEngine;

public class UpgradeTreeUI : MonoBehaviour
{
    public static UpgradeTreeUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private UpgradeTreeData _data;
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private Transform _nodesContainer;
    [SerializeField] private LineRenderer _connectionLinePrefab;

    [Header("Auto Refresh")]
    [SerializeField] private float _refreshInterval = 0.5f;

    private readonly Dictionary<string, UpgradeNodeUI> _nodeUIs = new();
    private float _refreshTimer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnNodes();
        SpawnConnections();
    }

    void Update()
    {
        _refreshTimer -= Time.deltaTime;
        if (_refreshTimer <= 0f)
        {
            RefreshAll();
            _refreshTimer = _refreshInterval;
        }
    }

    // ── Spawn ─────────────────────────────────────────────────────────────────

    private void SpawnNodes()
    {
        foreach (UpgradeNodeDefinition node in _data.nodes)
        {
            GameObject go = Instantiate(_nodePrefab, _nodesContainer);
            go.name = node.id;
            go.transform.localPosition = node.treePosition;

            UpgradeNodeUI ui = go.GetComponent<UpgradeNodeUI>();
            ui.Initialize(node);
            _nodeUIs[node.id] = ui;
        }
    }

    private void SpawnConnections()
    {
        foreach (UpgradeNodeDefinition node in _data.nodes)
        {
            if (string.IsNullOrEmpty(node.prerequisiteId)) continue;
            if (!_nodeUIs.TryGetValue(node.prerequisiteId, out UpgradeNodeUI parent)) continue;
            if (!_nodeUIs.TryGetValue(node.id, out UpgradeNodeUI child)) continue;

            LineRenderer line = Instantiate(_connectionLinePrefab, _nodesContainer);
            line.positionCount = 2;
            line.SetPosition(0, parent.transform.position);
            line.SetPosition(1, child.transform.position);
        }
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    public void RefreshAll()
    {
        foreach (UpgradeNodeUI ui in _nodeUIs.Values)
            ui.Refresh();
    }
}
