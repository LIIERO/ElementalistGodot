using Godot;
using System;

public partial class ElementCharge : Area2D
{
	public bool IsActivated { get; private set; } = false;
    private Player playerInRange = null;

    // Singletons
    private CustomSignals customSignals;
    private AudioManager audioManager;

    private AnimatedSprite2D sprite;
    private PointLight2D light;

    public override void _Ready()
	{
        sprite = GetNode<AnimatedSprite2D>("Sprite2D");
        light = GetNode<PointLight2D>("PointLight2D");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
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

    void _OnAreaEntered(Area2D fireball)
    {
        if (fireball is Fireball)
        {
            Activate();
        }
    }

    private void Activate()
    {
        if (IsActivated) return;
        IsActivated = true;
        sprite.Play("Activated");
        audioManager.elementCharge.Play();
        light.Show();
        customSignals.EmitSignal(CustomSignals.SignalName.ElementChargeActivated);
    }
}
