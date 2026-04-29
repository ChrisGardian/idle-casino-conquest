using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineLineUI : MonoBehaviour
{
    [Header("Line Reference")]
    [SerializeField] private MachineLine _line;

    [Header("Management Panel")]
    [SerializeField] private GameObject _managementPanel;
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _editButton;

    [Header("Popups")]
    [SerializeField] private MachineLineInfoPopup _infoPopup;
    [SerializeField] private MachineLineEditPopup _editPopup;

    void Awake()
    {
        _infoButton.onClick.AddListener(() => _infoPopup.Open(_line));
        _editButton.onClick.AddListener(() => _editPopup.Open(_line));
    }

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        _managementPanel.SetActive(_line.IsUnlocked);
    }
}
