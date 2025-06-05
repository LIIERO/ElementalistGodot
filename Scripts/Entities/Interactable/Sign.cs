using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class Sign : Interactable
{
    private CustomSignals customSignals; // Singleton

    [Export] public string Text { get; set; }
    [Export] public string SunHoldingText { get; set; }

    // TODO bool checkbox whether dialog should be on top or bottom

    public override void _Ready()
	{
		base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    protected override void Interact()
	{
		base.Interact();

        // This middle bit is only for easter eggs where sign says something different when you carry a sun
        string text = Text;
        if (playerScriptReference.IsHoldingGoal)
        {
            text = string.IsNullOrEmpty(SunHoldingText) ? Text : SunHoldingText;
        }

        customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, text);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
    }
}
