using Godot;
using System;

public partial class WatchtowerFrame : Sprite2D
{
	[Export] private Node2D arrowLeft;
    [Export] private Node2D arrowRight;

    private GameState gameState; // Singleton

	//private bool previousWatchtowerState = false;

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if (!previousWatchtowerState && gameState.WatchtowerActive) {

		//previousWatchtowerState = gameState.WatchtowerActive;

		if (!gameState.WatchtowerActive)
		{
			Hide();
			return;
		}

		arrowLeft.Show();
        arrowRight.Show();

        if (gameState.CameraTouchingBorders.left)
			arrowLeft.Hide();
        if (gameState.CameraTouchingBorders.right)
            arrowRight.Hide();

        Show();
	}
}
