using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class LibraryShelf : Interactable
{
    private CustomSignals customSignals; // Singleton

    public bool Locked { get; set; } = true;

    [Export] private string lockedText;

    //private Node2D unlockImages;
    //private Sprite2D lockSprite;

    public override void _Ready()
	{
		base._Ready();

        if (Locked) GetNode<Node2D>("Images").Hide();
        else GetNode<Sprite2D>("Lock").Hide();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    protected override void Interact()
	{
		base.Interact();

        if (Locked)
        {
            customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, lockedText);
            return;
        }

        GD.Print("TODO: Library knowledge.");
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
    }
}
