using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _volumePanel;
    [SerializeField] private Slider     _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeLabel;

    private bool _panelOpen = false;

    private void Start()
    {
        if (AudioManager.Instance != null)
            _volumeSlider.value = AudioManager.Instance.GetMasterVolume();

        _volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        _volumePanel.SetActive(false);
        UpdateLabel();
    }

    public void TogglePanel()
    {
        _panelOpen = !_panelOpen;
        _volumePanel.SetActive(_panelOpen);
    }

    private void OnSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);

        UpdateLabel();
    }

    private void UpdateLabel()
    {
        _volumeLabel.text = $"{Mathf.RoundToInt(_volumeSlider.value * 100)} %";
    }
}