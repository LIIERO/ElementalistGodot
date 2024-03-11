using Godot;
using System;

public partial class ShiftCameraLimit : Area2D
{
    private CustomSignals customSignals; // Singleton

    [Export] int leftShiftUnits = 0;
    [Export] int rightShiftUnits = 0;

    private bool triggered = false;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

	void _OnBodyEntered(Node2D player)
	{
        if (player is not Player || triggered) return;
        triggered = true;
        customSignals.EmitSignal(CustomSignals.SignalName.ShiftCameraXLimits, leftShiftUnits, rightShiftUnits);
    }
}
