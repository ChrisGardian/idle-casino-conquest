using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MachineLineInfoPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _closeButton;

    public static bool IsUnlocked { get; private set; } = false;
    public static event System.Action OnUnlocked;

    public static void Unlock()
    {
        IsUnlocked = true;
        OnUnlocked?.Invoke();
    }

    private MachineLine _line;
    private float _refreshTimer;
    private bool _skipCloseFrame;
    private const float RefreshInterval = 1f;

    void Awake()
    {
        _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (_skipCloseFrame) { _skipCloseFrame = false; }
        else if (Mouse.current.leftButton.wasPressedThisFrame && !RectTransformUtility.RectangleContainsScreenPoint(
            (RectTransform)transform, Mouse.current.position.ReadValue(), null))
        {
            gameObject.SetActive(false);
            return;
        }

        _refreshTimer += Time.deltaTime;
        if (_refreshTimer >= RefreshInterval)
        {
            _refreshTimer = 0f;
            Refresh();
        }
    }

    public void Open(MachineLine line)
    {
        if (!IsUnlocked) return;
        _line = line;
        _refreshTimer = RefreshInterval;
        _skipCloseFrame = true;
        gameObject.SetActive(true);
    }

    private void Refresh()
    {
        _infoText.text =
            $"<b>Machines</b>  {_line.MachineCount}\n" +
            $"<b>Level</b>  {_line.UpgradeLevel}\n" +
            $"<b>Active NPCs</b>  {_line.TotalNPCsOnLine}\n" +
            $"<b>Total Revenue</b>  {_line.TotalRevenue:F0}$\n" +
            $"<b>Revenue/min</b>  {_line.TotalRevenuePerMinute:F1}$/min";
    }
}
