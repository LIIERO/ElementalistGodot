using Godot;
using System;
using System.Collections.Generic;

public partial class ChargeGate : Gate, IUndoable
{

    [Export] private int requiredCharges;
    private int noCharges = 0;

    // Undo system
    private List<int> chargeGateCountCheckpoints = new List<int>();

    public override void _Ready()
	{
        base._Ready();

        customSignals.Connect(CustomSignals.SignalName.ElementChargeActivated, new Callable(this, MethodName.AddCharge));
        requiredFragmentsDisplay.Text = requiredCharges.ToString();
    }

    private void AddCharge()
    {
        noCharges++;

        if (!isOpened && noCharges >= requiredCharges)
        {
            Open();
        }
    }

    public override void AddLocalCheckpoint()
    {
        base.AddLocalCheckpoint();
        chargeGateCountCheckpoints.Add(noCharges);
    }

    public override void UndoLocalCheckpoint(bool nextCpRequested)
    {
        base.UndoLocalCheckpoint(nextCpRequested);

        if (!nextCpRequested && chargeGateCountCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(chargeGateCountCheckpoints);
        noCharges = chargeGateCountCheckpoints[^1];
    }

    public override void ReplaceTopLocalCheckpoint()
    {
        base.ReplaceTopLocalCheckpoint();
        GameUtils.ListRemoveLastElement(chargeGateCountCheckpoints);
        chargeGateCountCheckpoints.Add(noCharges);
    }
}
