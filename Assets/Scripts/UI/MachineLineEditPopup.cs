using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineLineEditPopup : MonoBehaviour
{
    [SerializeField] private Slider _payoutSlider;
    [SerializeField] private TextMeshProUGUI _payoutValueText;
    [SerializeField] private Button _closeButton;

    private MachineLine _line;

    void Awake()
    {
        _payoutSlider.minValue = 0f;
        _payoutSlider.maxValue = 1f;
        _payoutSlider.onValueChanged.AddListener(OnSliderChanged);
        _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    public void Open(MachineLine line)
    {
        _line = line;

        float defaultRate = line.data.machinePrefab.GetComponent<Machine>().data.payoutRate;
        float initialValue = line.UsePayoutOverride ? line.PayoutOverride : defaultRate;
        _payoutSlider.SetValueWithoutNotify(initialValue);

        RefreshLabel();
        gameObject.SetActive(true);
    }

    private void OnSliderChanged(float value)
    {
        _line.SetPayoutOverride(value);
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        _payoutValueText.text = $"{_payoutSlider.value * 100f:F0}%";
    }
}
