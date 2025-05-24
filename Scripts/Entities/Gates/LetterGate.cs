using Godot;
using System;

public partial class LetterGate : Gate, IUndoable
{

    private float detectionRangeUnits = 7.0f;
    [Export] private string requiredLetter;
    [Export] Node2D playerNode;

    private AnimatedSprite2D letterDisplay;

    public override void _Ready()
	{
        base._Ready();

        letterDisplay = GetNode<AnimatedSprite2D>("ToMove/Sprite2D/LetterDisplay");
        letterDisplay.Play(requiredLetter);
        requiredFragmentsDisplay.QueueFree();
        detectionRangeUnits *= GameUtils.gameUnitSize;
    }

	public override void _Process(double delta)
	{
        base._Process(delta);

        if (Position.DistanceTo(playerNode.Position) > detectionRangeUnits) return;

        if (!isOpened && gameState.UnlockedLetters.Contains(requiredLetter))
        {
            Open();
        }
    }
}
