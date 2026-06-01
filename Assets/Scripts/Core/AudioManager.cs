using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources audio")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _ambianceSource;

    [Header("Volume ambiance (scale with NPCs)")]
    [SerializeField] private float _maxNPCsForFullVolume = 150f;

    private const string MasterVolumeKey = "MasterVolume";
    private float _masterVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        ApplyMasterVolume();
    }

    private void Update()
    {
        if (_ambianceSource == null || NPCManagementSystem.Instance == null) return;

        float npcRatio = Mathf.Clamp01(NPCManagementSystem.Instance.numberOfNPCs / _maxNPCsForFullVolume);
        _ambianceSource.volume = Mathf.Lerp(0.05f, 0.8f, npcRatio) * _masterVolume;
    }

    public float GetMasterVolume() => _masterVolume;

    public void SetMasterVolume(float value)
    {
        _masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, _masterVolume);
        ApplyMasterVolume();
    }

    private void ApplyMasterVolume()
    {
        AudioListener.volume = _masterVolume;
        if (_musicSource != null)
            _musicSource.volume = _masterVolume;
    }
}
