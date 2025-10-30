using Godot;
using System;
using System.Linq;

public partial class EarthBlockPuzzleChecker : Sprite2D
{
    [Export] private EmptyGate gate;
    [Export] private Node2D[] blockPositions;
    [Export] private int[] hasToBeDark; // 0 - false, 1 - true

    private const double checkTime = 0.5;
    private double checkCounter = 0;

	public override void _Ready()
	{
	}


	public override void _Process(double delta)
	{
        checkCounter += delta;
        if (checkCounter > checkTime)
        {
            CheckSuccess();
            checkCounter = 0;
        }
	}

    private void CheckSuccess()
    {
        if (gate.isOpened) return;

        bool success = false;
        var blocks = GetTree().GetNodesInGroup("EarthBlock");
        //GD.Print($"\nAttempt, blocks: {blocks.Count}");

        if (blocks.Count != blockPositions.Count())
            return;

        foreach (EarthBlock block in blocks)
        {
            for (int i = 0; i < blockPositions.Length; i++)
            {
                bool darkRequired = hasToBeDark[i] == 1 ? true : false;
                //GD.Print($"block Position: {block.GlobalPosition}, required {blockPositions[j].GlobalPosition}");
                if (block.GlobalPosition == blockPositions[i].GlobalPosition && block.IsDark == darkRequired)
                {
                    //GD.Print("one success");
                    success = true;
                    break;
                }
                else
                {
                    success = false;
                }
            }

            if (!success) return;
        }

        if (success) gate.Unlock();
    }
}
