using Godot;
using System;
using System.Collections.Generic;

public partial class ElementCharge : Area2D, IUndoable
{
	public bool IsActivated { get; private set; } = false;
    private Player playerInRange = null;

    private bool lightActive;

    // Singletons
    private CustomSignals customSignals;
    private AudioManager audioManager;

    private AnimatedSprite2D sprite;
    private PointLight2D light;

    // Undo system
    private List<bool> chargeStateCheckpoints = new List<bool>();

    public override void _Ready()
	{
        sprite = GetNode<AnimatedSprite2D>("Sprite2D");
        light = GetNode<PointLight2D>("PointLight2D");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.ReplaceTopCheckpoint, new Callable(this, MethodName.ReplaceTopLocalCheckpoint));
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

        lightActive = GetNode<SettingsManager>("/root/SettingsManager").LightParticlesActive;
        if (!lightActive)
        {
            light.QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        if (playerInRange == null) return;

        if (playerInRange.isUsingAbility)
        {
            Activate();
        }
    }

    void _OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            playerInRange = body as Player;
        }
        
        if (body is Fireball)
        {
            Activate();
        }
    }

    void _OnBodyExited(Node2D body)
    {
        if (body is Player)
        {
            playerInRange = null;
        }
    }

    private void Activate()
    {
        if (IsActivated) return;
        IsActivated = true;
        sprite.Play("Activated");
        audioManager.elementCharge.Play();
        if (lightActive) light.Show();
        customSignals.EmitSignal(CustomSignals.SignalName.ElementChargeActivated);
    }

    private void Deactivate()
    {
        IsActivated = false;
        sprite.Play("Default");
        light.Hide();
    }

    public void AddLocalCheckpoint()
    {
        chargeStateCheckpoints.Add(IsActivated);
    }

    public void UndoLocalCheckpoint(bool nextCpRequested)
    {
        if (!nextCpRequested && chargeStateCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(chargeStateCheckpoints);
        IsActivated = chargeStateCheckpoints[^1];

        if (!IsActivated) Deactivate();
    }

    public void ReplaceTopLocalCheckpoint()
    {
        GameUtils.ListRemoveLastElement(chargeStateCheckpoints);
        AddLocalCheckpoint();
    }
}
