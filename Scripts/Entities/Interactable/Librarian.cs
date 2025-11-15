using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class Librarian : Interactable
{
    private CustomSignals customSignals; // Singleton

    [Export] private Node2D playerNode;
    [Export] private string librarianMissingText;

    private bool librarianMissing = false;

    private AnimatedSprite2D tableAnimation;
    private AnimatedSprite2D librarianAnimation;

    public override void _Ready()
	{
		base._Ready();
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        tableAnimation = GetNode<AnimatedSprite2D>("Body/Table");
        librarianAnimation = GetNode<AnimatedSprite2D>("Body/Librarian");

        if (librarianMissing)
        {
            tableAnimation.Play("Idle");
            librarianAnimation.Hide();
            return;
        }

        tableAnimation.Play("Sleeping");
        librarianAnimation.Play("Idle");
    }

    public override void _Process(double delta)
    {
        if (librarianMissing) return;

        if (playerNode.GlobalPosition.X > GlobalPosition.X)
            librarianAnimation.FlipH = true;
        else
            librarianAnimation.FlipH = false;
    }

    protected override void Interact()
	{
		base.Interact();

        customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, librarianMissingText);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
    }
}
