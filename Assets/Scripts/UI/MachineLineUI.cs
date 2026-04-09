using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineLineUI : MonoBehaviour
{
    [Header("Line Reference")]
    [SerializeField] private MachineLine _line;

    [Header("Unlock Panel")]
    [SerializeField] private GameObject _unlockPanel;
    [SerializeField] private TextMeshProUGUI _unlockCostText;
    [SerializeField] private Button _unlockButton;

    [Header("Management Panel")]
    [SerializeField] private GameObject _managementPanel;
    [SerializeField] private Button _addMachineButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _editButton;

    void Awake()
    {
        _unlockButton.onClick.AddListener(OnUnlockClicked);
        if (_addMachineButton != null) _addMachineButton.onClick.AddListener(OnAddMachineClicked);
        if (_upgradeButton != null) _upgradeButton.onClick.AddListener(OnUpgradeClicked);
        if (_editButton != null) _editButton.onClick.AddListener(OnEditClicked);
    }

    void Start()
    {
        Refresh();
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    private void Refresh()
    {
        bool isUnlocked = _line.IsUnlocked;

        _unlockPanel.SetActive(!isUnlocked);
        _managementPanel.SetActive(isUnlocked);

        if (!isUnlocked)
        {
            _unlockCostText.text = $"{_line.data.unlockCost:F0}$";
            _unlockButton.interactable = _line.CanUnlock();
        }
        else
        {
            _addMachineButton.interactable = _line.CanAddMachine();
            _upgradeButton.interactable = _line.CanUpgrade();
        }
    }

    // ── Button Callbacks ──────────────────────────────────────────────────────

    private void OnUnlockClicked()
    {
        _line.TryUnlock();
        Refresh();
    }

    private void OnAddMachineClicked()
    {
        _line.AddMachine();
        Refresh();
    }

    private void OnUpgradeClicked()
    {
        _line.TryUpgrade();
        Refresh();
    }

    private void OnEditClicked()
    {
        // Step 5 : popup de configuration
    }
}
