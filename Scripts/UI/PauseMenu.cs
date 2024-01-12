using Godot;
using System;

public partial class PauseMenu : Panel
{
    // Singletons
    private CustomSignals customSignals;
    private GameState gameState;

	public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        Resume();
	}

    public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("inputPause"))
		{
			if (gameState.IsGamePaused)
				Resume();
			else
				Pause();
		}
	}


    public void Pause()
	{
		Engine.TimeScale = 0;
		GetTree().Paused = true;
		Show();
        gameState.IsGamePaused = true;
	}

	public void Resume()
	{
        Engine.TimeScale = 1;
        GetTree().Paused = false;
        Hide();
        gameState.IsGamePaused = false;
    }
}
