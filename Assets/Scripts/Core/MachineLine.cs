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

    [Header("State")]
    [SerializeField] private bool _isUnlocked = false;
    [SerializeField] private int _upgradeLevel = 0;

    public bool IsUnlocked => _isUnlocked;
    public int UpgradeLevel => _upgradeLevel;
    public IReadOnlyList<Machine> Machines => _machines;
    public int MaxMachines => data.maxMachinesPerLevel != null && _upgradeLevel < data.maxMachinesPerLevel.Length
        ? data.maxMachinesPerLevel[_upgradeLevel]
        : 1;

    public float RevenueMultiplier => data.revenueMultiplierPerLevel != null && _upgradeLevel < data.revenueMultiplierPerLevel.Length
        ? data.revenueMultiplierPerLevel[_upgradeLevel]
        : 1f;

    private readonly List<Machine> _machines = new();

    void Awake()
    {
        MachineLineManager.Instance.RegisterLine(this);
    }

    void Start()
    {
        if (!_isUnlocked)
            SpawnMachine(locked: true);
    }

    void OnDestroy()
    {
        MachineLineManager.Instance?.UnregisterLine(this);
    }

    // ── Unlock ────────────────────────────────────────────────────────────────

    public bool CanUnlock() => !_isUnlocked && CurrencyManager.Instance.money >= data.unlockCost;

    public bool TryUnlock()
    {
        if (!CurrencyManager.Instance.TrySpendMoney(data.unlockCost)) return false;
        _isUnlocked = true;

        // La machine preview devient active
        if (_machines.Count > 0)
            _machines[0].SetLocked(false);

        return true;
    }

    // ── Add Machine ───────────────────────────────────────────────────────────

    public bool CanAddMachine() => _isUnlocked && _machines.Count < MaxMachines;

    public void AddMachine()
    {
        if (!CanAddMachine()) return;
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
        GameObject go = Instantiate(data.machinePrefab, spawnPos, Quaternion.identity);
        Machine machine = go.GetComponent<Machine>();
        machine.SetLine(this);
        machine.SetLocked(locked);
        _machines.Add(machine);
    }

    private Vector3 GetNextMachinePosition()
    {
        Vector3 origin = _origin != null ? _origin.position : transform.position;
        return origin + Vector3.right * (_machines.Count * _machineSpacing);
    }
}
