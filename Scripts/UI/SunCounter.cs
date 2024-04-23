using Godot;
using System;

public partial class SunCounter : Sprite2D
{
    [Export] private bool specialCounter = false;
	[Export] private Label counterText;

    private GameState gameState;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");

        if (!gameState.IsHubLoaded()) Hide();

        if (specialCounter)
            counterText.Text = gameState.NoRedFragments.ToString();
        else
            counterText.Text = gameState.NoSunFragments.ToString();
    }
}
