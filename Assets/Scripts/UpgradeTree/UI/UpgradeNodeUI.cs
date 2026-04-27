using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _button;

    [Header("State Colors")]
    [SerializeField] private Color _lockedColor   = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private Color _affordableColor = new Color(0.2f, 0.7f, 0.3f);
    [SerializeField] private Color _expensiveColor  = new Color(0.8f, 0.6f, 0.1f);
    [SerializeField] private Color _maxedColor      = new Color(0.5f, 0.3f, 0.8f);

    private string _nodeId;

    public string NodeId => _nodeId;

    // ── Init ──────────────────────────────────────────────────────────────────

    public void Initialize(UpgradeNodeDefinition node)
    {
        _nodeId = node.id;

        _nameText.text = node.displayName;

        if (_icon != null)
            _icon.sprite = node.icon;

        _button.onClick.AddListener(OnClick);

        Refresh();
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    public void Refresh()
    {
        UpgradeTreeManager mgr = UpgradeTreeManager.Instance;
        UpgradeNodeDefinition node = mgr.GetNodeDefinition(_nodeId);

        bool accessible = mgr.IsAccessible(_nodeId);
        bool canAct     = mgr.CanAct(_nodeId);
        int  level      = mgr.GetLevel(_nodeId);
        bool maxed      = level >= node.maxLevel;

        // Level label
        if (node.nodeType == NodeType.Unlock)
            _levelText.text = level > 0 ? "Unlocked" : "Locked";
        else
            _levelText.text = $"{level} / {node.maxLevel}";

        // Cost label
        if (!accessible)
            _costText.text = "???";
        else if (maxed)
            _costText.text = "MAX";
        else
            _costText.text = FormatCost(mgr.GetCost(node));

        // Background color
        if (!accessible)
            _background.color = _lockedColor;
        else if (maxed)
            _background.color = _maxedColor;
        else if (canAct)
            _background.color = _affordableColor;
        else
            _background.color = _expensiveColor;

        // Interactable
        _button.interactable = canAct;
    }

    // ── Click ─────────────────────────────────────────────────────────────────

    private void OnClick()
    {
        if (UpgradeTreeManager.Instance.TryAct(_nodeId))
            UpgradeTreeUI.Instance.RefreshAll();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string FormatCost(float cost)
    {
        if (cost >= 1_000_000_000f) return $"{cost / 1_000_000_000f:0.#}B$";
        if (cost >= 1_000_000f)     return $"{cost / 1_000_000f:0.#}M$";
        if (cost >= 1_000f)         return $"{cost / 1_000f:0.#}K$";
        return $"{cost:0}$";
    }
}
