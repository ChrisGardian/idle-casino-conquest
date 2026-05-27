using TMPro;
using UnityEngine;

/// <summary>
/// Texte flottant qui monte et disparaît progressivement.
/// Vert si le montant est positif (casino gagne), rouge si négatif (casino perd).
/// Spawné par FloatingTextSpawner — ne pas instancier directement.
/// </summary>
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _duration  = 1.5f;
    [SerializeField] private float _riseSpeed = 0.8f;

    private float _timer;
    private Color _startColor;

    public void Setup(float amount)
    {
        if (_text == null)
        {
            Debug.LogError("[FloatingText] _text est null ! Le champ TextMeshPro n'est pas assigné sur le prefab.");
            return;
        }

        string sign = amount >= 0f ? "+" : "";
        _text.text = $"{sign}{amount:F1}$";

        _startColor = amount >= 0f
            ? new Color(0.15f, 0.85f, 0.15f)   // vert vif
            : new Color(0.95f, 0.15f, 0.15f);   // rouge vif
        _text.color = _startColor;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        transform.position += Vector3.up * _riseSpeed * Time.deltaTime;

        float alpha = Mathf.Clamp01(1f - _timer / _duration);
        _text.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);

        if (_timer >= _duration)
            Destroy(gameObject);
    }
}
