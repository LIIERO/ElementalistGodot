using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class Librarian : Interactable
{
    private CustomSignals customSignals; // Singleton

    [Export] public string Text { get; set; } // TODO: rename to dialogID, make sign into an npc class to handle this for every single npc
    [Export] public string SunHoldingText { get; set; }

    private AnimatedSprite2D tableAnimation;
    private AnimatedSprite2D librarianAnimation;

    public override void _Ready()
	{
		base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        tableAnimation = GetNode<AnimatedSprite2D>("Body/Table");
        librarianAnimation = GetNode<AnimatedSprite2D>("Body/Librarian");
        tableAnimation.Play("SleepingIdle");
        librarianAnimation.Play("Idle");
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
