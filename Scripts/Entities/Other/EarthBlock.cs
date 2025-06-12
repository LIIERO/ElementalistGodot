using Godot;
using System.Collections.Generic;
using System;

public partial class EarthBlock : RigidBody2D, IUndoable
{
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private int noAliveCheckpoints = 0;
    private float velocityY = 0.0f;

    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
    }

	public override void _PhysicsProcess(double delta)
	{
        velocityY += gravity * (float)delta;
        SetAxisVelocity(new Vector2(0f, velocityY));
    }

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
}
