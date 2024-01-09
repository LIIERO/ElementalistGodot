using Godot;
using System;

public partial class Sign : Interactable
{
    // Signals
    private CustomSignals customSignals;

    [Export] public string Text { get; set; }

    public override void _Ready()
	{
		base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    protected override void Interact()
	{
		base.Interact();

        customSignals.EmitSignal(CustomSignals.SignalName.DialogBoxShow, Text);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.DialogBoxHide);
    }
}
