using Godot;
using System;

public partial class GamepadNote : Label
{
    protected GameState gameState;
    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        Text = gameState.UITextData["gamepad_note"];
    }
}
