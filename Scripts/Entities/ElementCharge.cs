using Godot;
using System;

public partial class ElementCharge : Area2D
{
	public bool IsActivated { get; private set; } = false;
    private Player playerInRange = null;

    private CustomSignals customSignals; // Singleton
    private AnimatedSprite2D sprite;

    public override void _Ready()
	{
        sprite = GetNode<AnimatedSprite2D>("Sprite2D");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    public override void _Process(double delta)
    {
        if (playerInRange == null) return;

        if (playerInRange.isUsingAbility)
        {
            Activate();
        }
    }

    void _OnBodyEntered(Node2D player)
    {
        if (player is not Player) return;
        playerInRange = player as Player;
    }

    void _OnBodyExited(Node2D player)
    {
        if (player is not Player) return;
        playerInRange = null;
    }

    private void Activate()
    {
        if (IsActivated) return;
        IsActivated = true;
        sprite.Play("Activated");
        customSignals.EmitSignal(CustomSignals.SignalName.ElementChargeActivated);
    }
}
