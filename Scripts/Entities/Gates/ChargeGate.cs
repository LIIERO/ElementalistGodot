using Godot;
using System;

public partial class ChargeGate : Gate
{

    [Export] private int requiredCharges;
    private int noCharges = 0;

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
}
