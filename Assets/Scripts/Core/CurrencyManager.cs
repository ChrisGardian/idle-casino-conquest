using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set;}

    [Header("Currencies")]
    public float money = 1000f;
    public float popularity = 2f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use with negative value for NPCs (they can bring you in negative by winning)
    public void AddMoney(float amount) => money += amount;

    // Use for Upgrades (the Player can't go in the negatives while trying to purchase a upgrade)
    public bool TrySpendMoney(float amount)
    {
        if (money < amount) return false;
        money -= amount;
        return true;
    }

    public void AddPopularity(float amount)
    { 
        popularity = Mathf.Max(1f, popularity + amount);
    }
}
