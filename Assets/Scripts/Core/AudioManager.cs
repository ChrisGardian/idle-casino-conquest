using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sources audio")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _ambianceSource;

    [Header("Volume ambiance (scale with NPCs)")]
    [SerializeField] private float _maxNPCsForFullVolume = 150f;

    private void Update()
    {
        if (_ambianceSource == null || NPCManagementSystem.Instance == null) return;

        float npcRatio = Mathf.Clamp01(NPCManagementSystem.Instance.numberOfNPCs / _maxNPCsForFullVolume);
        _ambianceSource.volume = Mathf.Lerp(0.05f, 0.8f, npcRatio);
    }
}
