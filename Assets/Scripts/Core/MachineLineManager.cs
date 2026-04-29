using System.Collections.Generic;
using UnityEngine;

public class MachineLineManager : MonoBehaviour
{
    public static MachineLineManager Instance { get; private set; }

    private readonly List<MachineLine> _lines = new();

    public IReadOnlyList<MachineLine> Lines => _lines;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterLine(MachineLine line) => _lines.Add(line);

    public void UnregisterLine(MachineLine line) => _lines.Remove(line);

    public MachineLine FindLine(MachineLineData data) => _lines.Find(l => l.data == data);
}
