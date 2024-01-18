using Godot;
using System;

public partial class SunCounter : Sprite2D
{
	[Export] private Label counterText;

    private GameState gameState;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");

        counterText.Text = gameState.NoCompletedLevels.ToString();
    }
}
