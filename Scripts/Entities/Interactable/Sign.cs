using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class Sign : Interactable
{
    private CustomSignals customSignals; // Singleton

    [Export] public string Text { get; set; } // TODO: rename to dialogID, make sign into an npc class to handle this for every single npc
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
            Text = string.IsNullOrEmpty(SunHoldingText) ? Text : SunHoldingText;
        }

        customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, Text);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
    }
}
