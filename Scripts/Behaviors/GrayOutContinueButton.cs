using Godot;
using System;

public partial class GrayOutContinueButton : Sprite2D
{
	private GameState gameState;

	public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
		if (!gameState.SaveFileExists("0"))
		{
			Modulate = new Color(1f, 1f, 1f, 0.5f);
		}
    }
}
