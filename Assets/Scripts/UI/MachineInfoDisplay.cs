using System.Text;
using TMPro;
using UnityEngine;

public class MachineInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private float refreshInterval = 0.5f;

    private Machine _machine;
    private StringBuilder _sb = new();

    void Awake()
    {
        _machine = GetComponentInParent<Machine>();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        InvokeRepeating(nameof(Refresh), 0f, refreshInterval);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        CancelInvoke(nameof(Refresh));
    }

    private void Refresh()
    {
        _sb.Clear();

        // Ligne titre : nom + slots
        _sb.AppendLine($"{_machine.name}  {_machine.Occupants.Count}/{_machine.data.totalSlots}");

        // Un NPC par ligne avec son temps restant sur la machine
        if (_machine.Occupants.Count == 0)
        {
            _sb.AppendLine("Vide");
        }
        else
        {
            foreach (NPC npc in _machine.Occupants)
                _sb.AppendLine($"{npc.name}  {npc.PatienceMachineRemaining:F1}s");
        }

        // Revenu/min du casino sur cette machine
        float rpm = _machine.RevenuePerMinute;
        string sign = rpm >= 0f ? "+" : "";
        _sb.Append($"Rev: {sign}{rpm:F0}/min");

        infoText.text = _sb.ToString();
    }
}
