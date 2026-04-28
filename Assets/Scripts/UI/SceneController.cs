using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [SerializeField] private Button _upgradeTreeButton;
    [SerializeField] private PanZoomController _panZoom;
    [SerializeField] private Color _buttonActiveColor = new Color(0.4f, 0.85f, 1f);

    private bool _isUpgradeTreeOpen;
    private Color _buttonDefaultColor;
    private const string UpgradeTreeScene = "UpgradeTreeScene";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _buttonDefaultColor = _upgradeTreeButton.GetComponent<Image>().color;
        _upgradeTreeButton.onClick.AddListener(ToggleUpgradeTree);
    }

    void Update()
    {
        if (_isUpgradeTreeOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseUpgradeTree();
    }

    public void ToggleUpgradeTree()
    {
        if (_isUpgradeTreeOpen)
            CloseUpgradeTree();
        else
            OpenUpgradeTree();
    }

    void OpenUpgradeTree()
    {
        _isUpgradeTreeOpen = true;
        if (_panZoom != null) _panZoom.enabled = false;
        SceneManager.LoadScene(UpgradeTreeScene, LoadSceneMode.Additive);
        _upgradeTreeButton.GetComponent<Image>().color = _buttonActiveColor;
    }

    void CloseUpgradeTree()
    {
        _isUpgradeTreeOpen = false;
        if (_panZoom != null) _panZoom.enabled = true;
        SceneManager.UnloadSceneAsync(UpgradeTreeScene);
        _upgradeTreeButton.GetComponent<Image>().color = _buttonDefaultColor;
    }
}
