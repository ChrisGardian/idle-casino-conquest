using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Bouton de volume en haut à droite.
/// Attache ce script sur le GameObject racine du bouton volume.
///
/// Hiérarchie attendue dans le Canvas :
///   VolumeButton  (Button + ce script)
///     VolumeIcon  (TextMeshProUGUI — affiche 🔊 ou 🔇)
///     VolumePanel (GameObject désactivé par défaut)
///       VolumeSlider (Slider)
///       VolumeLabel  (TextMeshProUGUI — affiche "75 %")
/// </summary>
public class VolumeButtonUI : MonoBehaviour
{
    [Header("Références")]
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

    /// <summary>Appelé par le Button (OnClick).</summary>
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
