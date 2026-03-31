using UnityEngine;

public class HighRollerMachine : Machine
{
    public override float payoutRate => 0.40f;
    public override float betAmountMultiplier => 3f;
    public override float patienceWinGainMultiplier => 1.5f;
}