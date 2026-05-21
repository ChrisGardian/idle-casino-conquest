using System.Collections;
using UnityEngine;

public class CasinoDoor : MonoBehaviour
{
    public static CasinoDoor Instance { get; private set; }

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite[] _openFrames;
    [SerializeField] private float _frameDuration = 0.06f;

    private bool _isOpen = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        StartCoroutine(PlayOpenAnimation());
    }

    private IEnumerator PlayOpenAnimation()
    {
        foreach (Sprite frame in _openFrames)
        {
            _renderer.sprite = frame;
            yield return new WaitForSeconds(_frameDuration);
        }
    }
}
