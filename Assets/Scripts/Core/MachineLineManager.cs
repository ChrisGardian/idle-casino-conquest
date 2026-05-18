using System.Collections.Generic;
using UnityEngine;

public class MachineLineManager : MonoBehaviour
{
    public static MachineLineManager Instance { get; private set; }

    private readonly List<MachineLine> _lines = new();

    public IReadOnlyList<MachineLine> Lines => _lines;

    [Header("Debug")]
    [SerializeField] private bool _debugUnlockAll = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (!_debugUnlockAll) return;
        foreach (var line in _lines)
            if (!line.IsUnlocked) line.Unlock();
    }

    public void RegisterLine(MachineLine line) => _lines.Add(line);

    public void UnregisterLine(MachineLine line) => _lines.Remove(line);

    public MachineLine FindLine(MachineLineData data) => _lines.Find(l => l.data == data);
}
