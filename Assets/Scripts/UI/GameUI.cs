using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI popularityText;
    public TextMeshProUGUI npcCountText;

    void Update()
    {
        var cm = CurrencyManager.Instance;
        var nm = NPCManagementSystem.Instance;
        moneyText.text = $"Money: {cm.money:F0}$";
        popularityText.text = $"Popularity {cm.popularity:F1}";
        npcCountText.text = $"NPC count {nm.numberOfNPCs:F2}";
    }
}