using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineLine : MonoBehaviour
{
    [Header("Data")]
    public MachineLineData data;

    [Header("Placement")]
    [Tooltip("Point de départ du placement des machines (coin gauche de la ligne)")]
    [SerializeField] private Transform _origin;
    [SerializeField] private float _machineSpacing = 2.5f;
    [SerializeField] private Transform _machinesContainer;

    [Header("State")]
    [SerializeField] private bool _isUnlocked = false;
    [SerializeField] private int _upgradeLevel = 0;

    public event Action OnUnlocked;

    public bool IsUnlocked => _isUnlocked;
    public int UpgradeLevel => _upgradeLevel;
    public IReadOnlyList<Machine> Machines => _machines;
    public int MachineCount => _machines.Count;

    public float RevenueMultiplier => data.revenueMultiplierPerLevel != null && _upgradeLevel < data.revenueMultiplierPerLevel.Length
        ? data.revenueMultiplierPerLevel[_upgradeLevel]
        : 1f;

    public int TotalNPCsOnLine
    {
        get { int n = 0; foreach (var m in _machines) n += m.Occupants.Count; return n; }
    }

    public float TotalRevenue
    {
        get { float t = 0f; foreach (var m in _machines) t += m.TotalRevenue; return t; }
    }

    public float TotalRevenuePerMinute
    {
        get { float t = 0f; foreach (var m in _machines) t += m.RevenuePerMinute; return t; }
    }

    private readonly List<Machine> _machines = new();
    private float _bonusPayoutRate = 0f;
    private float _bonusSessionDuration = 0f;

    public float BonusPayoutRate => _bonusPayoutRate;
    public float BonusSessionDuration => _bonusSessionDuration;

    public void AddBonusPayoutRate(float bonus) => _bonusPayoutRate = Mathf.Clamp01(_bonusPayoutRate + bonus);
    public void AddBonusSessionDuration(float bonus) => _bonusSessionDuration += bonus;

    void Awake()
    {
        MachineLineManager.Instance.RegisterLine(this);
    }

    void Start()
    {
        SpawnMachine(locked: !_isUnlocked);
    }

    void OnDestroy()
    {
        MachineLineManager.Instance?.UnregisterLine(this);
    }

    // ── Unlock ────────────────────────────────────────────────────────────────

    public bool Unlock()
    {
        _isUnlocked = true;

        if (_machines.Count > 0)
            _machines[0].SetLocked(false);

        OnUnlocked?.Invoke();
        return true;
    }

    // ── Add Machine ───────────────────────────────────────────────────────────

    public void AddMachine() => SpawnMachine(locked: false);

    public void AddMachinesFromUpgrade(int count)
    {
        for (int i = 0; i < count; i++)
            SpawnMachine(locked: false);
    }

    // ── Upgrade ───────────────────────────────────────────────────────────────

    public bool CanUpgrade()
    {
        if (!_isUnlocked) return false;
        if (data.upgradeCosts == null || _upgradeLevel >= data.upgradeCosts.Length) return false;
        return CurrencyManager.Instance.money >= data.upgradeCosts[_upgradeLevel];
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade()) return false;
        if (!CurrencyManager.Instance.TrySpendMoney(data.upgradeCosts[_upgradeLevel])) return false;
        _upgradeLevel++;
        return true;
    }

    // ── Payout Override (configuration ligne) ────────────────────────────────

    private bool _usePayoutOverride = false;
    private float _payoutOverride = 0.35f;

    public bool UsePayoutOverride => _usePayoutOverride;
    public float PayoutOverride => _payoutOverride;

    public void SetPayoutOverride(float rate)
    {
        _payoutOverride = Mathf.Clamp01(rate);
        _usePayoutOverride = true;
    }

    public void ClearPayoutOverride() => _usePayoutOverride = false;

    // ── Internal ──────────────────────────────────────────────────────────────

    private void SpawnMachine(bool locked)
    {
        Vector3 spawnPos = GetNextMachinePosition();
        GameObject go = Instantiate(data.machinePrefab, spawnPos, Quaternion.identity, _machinesContainer);
        Machine machine = go.GetComponent<Machine>();
        machine.SetLine(this);
        machine.SetLocked(locked);
        _machines.Add(machine);
    }

    private Vector3 GetNextMachinePosition()
    {
        Vector3 origin = _origin != null ? _origin.position : transform.position;
        return origin + Vector3.up * (_machines.Count * _machineSpacing);
    }
}
