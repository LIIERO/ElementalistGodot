using Godot;
using System.Collections.Generic;
using System;

public partial class EarthBlock : Area2D, IUndoable
{
    //public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private int noAliveCheckpoints = 0;
    private List<bool> darkCheckpoints = new List<bool>();
    //private float velocityY = 0.0f;

    private CollisionShape2D blockCollider;
    private Sprite2D sprite;

    private CustomSignals customSignals;

    public bool IsEnabled { get; private set; } = false;
    public bool IsDark { get; private set; } = false; // For one PF puzzle

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.SwapEarthBlockColor, new Callable(this, MethodName.ToggleDark)); // For one PF puzzle

        blockCollider = GetNode<CollisionShape2D>("Block/CollisionShape2D");
        blockCollider.Disabled = true;

        sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Modulate = new Color(1f, 1f, 1f, 0.5f);
    }

	/*public override void _PhysicsProcess(double delta)
	{
        velocityY += gravity * (float)delta;
        SetAxisVelocity(new Vector2(0f, velocityY));
    }*/

    public void AddLocalCheckpoint()
    {
        darkCheckpoints.Add(IsDark);
        //noAliveCheckpoints++;
    }

    public void UndoLocalCheckpoint(bool nextCpRequested)
    {
        /*if (!nextCpRequested && noAliveCheckpoints > 0) noAliveCheckpoints--;
        if (noAliveCheckpoints == 0)
        {
            QueueFree();
        }*/

        if (!nextCpRequested && darkCheckpoints.Count > 0) GameUtils.ListRemoveLastElement(darkCheckpoints);
        if (darkCheckpoints.Count == 0)
        {
            QueueFree();
            return;
        }

        if (IsDark != darkCheckpoints[^1])
            ToggleDark();
        IsDark = darkCheckpoints[^1];
    }

    public void ReplaceTopLocalCheckpoint()
    {
        GameUtils.ListRemoveLastElement(darkCheckpoints);
        AddLocalCheckpoint();
    }

    void _OnBodyExited(Node2D player)
    {
        if (player is not Player) return;

        CallDeferred("EnableBlock");
    }

    private void EnableBlock()
    {
        IsEnabled = true;
        blockCollider.Disabled = false;
        sprite.Modulate = new Color(1f, 1f, 1f, 1f);
        customSignals.EmitSignal(CustomSignals.SignalName.UnlockCheckpointing);
    }

    public void ToggleDark() // For one PF puzzle
    {
        if (!IsEnabled) return;

        IsDark = !IsDark;
        if (IsDark)
        {
            sprite.Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            sprite.Modulate = new Color(1f, 1f, 1f, 1f);
        }
    }
}
