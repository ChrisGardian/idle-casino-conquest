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
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _editButton;

    [Header("Popups")]
    [SerializeField] private MachineLineInfoPopup _infoPopup;
    [SerializeField] private MachineLineEditPopup _editPopup;

    void Awake()
    {
        _unlockButton.onClick.AddListener(OnUnlockClicked);
        _infoButton.onClick.AddListener(() => _infoPopup.Open(_line));
        _editButton.onClick.AddListener(() => _editPopup.Open(_line));
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
    }

    // ── Button Callbacks ──────────────────────────────────────────────────────

    private void OnUnlockClicked()
    {
        _line.TryUnlock();
        Refresh();
    }
}
