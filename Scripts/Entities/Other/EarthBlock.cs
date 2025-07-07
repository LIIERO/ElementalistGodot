using Godot;
using System.Collections.Generic;
using System;

public partial class EarthBlock : Area2D, IUndoable
{
    //public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private int noAliveCheckpoints = 0;
    //private float velocityY = 0.0f;

    private CollisionShape2D blockCollider;
    private Sprite2D sprite;

    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));

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
        noAliveCheckpoints++;
    }

    public void UndoLocalCheckpoint(bool nextCpRequested)
    {
        if (!nextCpRequested && noAliveCheckpoints > 0) noAliveCheckpoints--;
        if (noAliveCheckpoints == 0)
        {
            QueueFree();
        }
    }

    public void ReplaceTopLocalCheckpoint()
    {
        /*noAliveCheckpoints--;
        AddLocalCheckpoint();*/
    }

    void _OnBodyExited(Node2D player)
    {
        if (player is not Player) return;

        CallDeferred("EnableBlock");
    }

    private void EnableBlock()
    {
        blockCollider.Disabled = false;
        sprite.Modulate = new Color(1f, 1f, 1f, 1f);
    }
}
