using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineLineInfoPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _closeButton;

    private MachineLine _line;
    private float _refreshTimer;
    private const float RefreshInterval = 1f;

    void Awake()
    {
        _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    void Update()
    {
        _refreshTimer += Time.deltaTime;
        if (_refreshTimer >= RefreshInterval)
        {
            _refreshTimer = 0f;
            Refresh();
        }
    }

    public void Open(MachineLine line)
    {
        _line = line;
        _refreshTimer = RefreshInterval;
        gameObject.SetActive(true);
    }

    private void Refresh()
    {
        _infoText.text =
            $"<b>Machines</b>  {_line.MachineCount} / {_line.MaxMachines}\n" +
            $"<b>Niveau</b>  {_line.UpgradeLevel}\n" +
            $"<b>NPCs actifs</b>  {_line.TotalNPCsOnLine}\n" +
            $"<b>Revenus totaux</b>  {_line.TotalRevenue:F0}$\n" +
            $"<b>Revenu/min</b>  {_line.TotalRevenuePerMinute:F1}$/min";
    }
}
