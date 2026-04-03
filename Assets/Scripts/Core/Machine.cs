using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Machine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    public MachineData data;

    public bool HasFreeSlot => _occupants.Count < data.totalSlots;
    public IReadOnlyList<NPC> Occupants => _occupants;
    public float RevenuePerMinute => _trackingTime > 1f ? _totalRevenue / _trackingTime * 60f : 0f;

    private readonly List<NPC> _occupants = new();
    private float _totalRevenue = 0f;
    private float _trackingTime = 0f;
    private MachineInfoDisplay _infoDisplay;

    public bool TryOccupy(NPC npc)
    {
        if (!HasFreeSlot) return false;
        _occupants.Add(npc);
        return true;
    }

    public void FreeSlot(NPC npc)
    {
        _occupants.Remove(npc);
        NPCManagementSystem.Instance.OnMachineSlotFreed(this);
    }

    // Retourne si le NPC a gagné et le montant concerné (mise * winMultiplier si victoire, mise si défaite)
    public (bool win, float amount) Play(float betAmount)
    {
        bool win = Random.value < data.payoutRate;
        float amount = win ? betAmount * data.winMultiplier : betAmount;

        float casinoNet = win ? betAmount * (1f - data.winMultiplier) : betAmount;
        _totalRevenue += casinoNet;

        return (win, amount);
    }

    void Start()
    {
        NPCManagementSystem.Instance.RegisterMachine(this);
        _infoDisplay = GetComponentInChildren<MachineInfoDisplay>(true);
    }

    void Update()
    {
        _trackingTime += Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_infoDisplay != null)
            _infoDisplay.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_infoDisplay != null)
            _infoDisplay.Hide();
    }
}
