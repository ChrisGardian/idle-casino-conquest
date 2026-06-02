using TMPro;
using UnityEngine;

public class UpgradeTreeBadge : MonoBehaviour
{
    [SerializeField] private GameObject _badgeRoot;
    [SerializeField] private TextMeshProUGUI _countText;

    void Start()
    {
        UpgradeTreeManager.Instance.OnAvailableCountChanged += Refresh;
        Refresh(UpgradeTreeManager.Instance.AvailableActionsCount);
    }

    void OnDestroy()
    {
        if (UpgradeTreeManager.Instance != null)
            UpgradeTreeManager.Instance.OnAvailableCountChanged -= Refresh;
    }

    private void Refresh(int count)
    {
        _badgeRoot.SetActive(count > 0);
        _countText.text = count.ToString();
    }
}
