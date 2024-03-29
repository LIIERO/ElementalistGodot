using Godot;
using System;

public partial class SunCounter : Sprite2D
{
	[Export] private Label counterText;

    private GameState gameState;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");

        if (!gameState.IsHubLoaded()) Hide();
        counterText.Text = gameState.NoSunFragments.ToString();
    }
}
