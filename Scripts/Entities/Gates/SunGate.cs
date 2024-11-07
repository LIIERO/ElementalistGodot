using Godot;
using System;

public partial class SunGate : Gate, IUndoable
{

    private float detectionRangeUnits = 7.0f;
    [Export] private int requiredFragments;
    [Export] Node2D playerNode;

    public override void _Ready()
	{
        base._Ready();

        requiredFragmentsDisplay.Text = requiredFragments.ToString();
        detectionRangeUnits *= GameUtils.gameUnitSize;
    }

	public override void _Process(double delta)
	{
        base._Process(delta);

        if (Position.DistanceTo(playerNode.Position) > detectionRangeUnits) return;

        if (!isOpened && gameState.NoSunFragments >= requiredFragments)
        {
            Open();
        }
    }
}
