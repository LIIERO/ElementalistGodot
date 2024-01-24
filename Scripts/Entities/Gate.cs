using Godot;
using System;

public partial class Gate : Node2D
{
    private GameState gameState; // Singleton
    private AnimationPlayer animator;
    private Label requiredFragmentsDisplay;

    private bool isOpened = false;
    private float detectionRangeUnits = 7.0f;
    [Export] private int requiredFragments;
    [Export] Node2D playerNode;

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");

        requiredFragmentsDisplay.Text = requiredFragments.ToString();
        detectionRangeUnits *= GameUtils.gameUnitSize;
    }

	public override void _Process(double delta)
	{
        if (Position.DistanceTo(playerNode.Position) > detectionRangeUnits) return;

        if (!isOpened && gameState.NoCompletedLevels >= requiredFragments)
        {
            isOpened = true;
            animator.Play("Open");
        }
    }
}
