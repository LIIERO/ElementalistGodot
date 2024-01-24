using Godot;
using System;

public partial class Sign : Interactable
{
    private CustomSignals customSignals; // Singleton

    [Export] public string Text { get; set; }
    [Export] public string SunHoldingText { get; set; }

    // TODO bool checkbox whether dialog should be on top or bottom
    // TODO text appearing animation

    public override void _Ready()
	{
		base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    protected override void Interact()
	{
		base.Interact();

        // This middle bit is only for easter eggs where sign says something different when you carry a sun
        if (playerScriptReference.IsHoldingGoal)
        {
            string t = string.IsNullOrEmpty(SunHoldingText) ? Text : SunHoldingText;
            customSignals.EmitSignal(CustomSignals.SignalName.DialogBoxShow, t);
            return;
        }

        customSignals.EmitSignal(CustomSignals.SignalName.DialogBoxShow, Text);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.DialogBoxHide);
    }
}
