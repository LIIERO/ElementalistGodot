using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{
    // Singletons
    //private CustomSignals customSignals;
    const float resumeDelay = 0.1f;

	public override void _Ready()
	{
		base._Ready();
        Resume();
	}

    public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("inputPause"))
		{
			if (gameState.IsGamePaused)
			{
                Resume();
            }	
			else
            {
                ResetButtons();
                Pause();
            }
		}

        if (Input.IsActionJustPressed("inputJump") && gameState.IsGamePaused)
        {
            if (CurrentButtonIndex == 0) // resume
            {
                DelayResume();
            }
            if (CurrentButtonIndex == 1) // main menu
            {
                Resume();
                // TODO save progress
                gameState.LoadMenu();
            }
            if (CurrentButtonIndex == 2) // exit
            {
                // TODO save progress
                GetTree().Quit();
            }
        }
    }

    public void Pause()
	{
        //Engine.TimeScale = 0;
        Show();
        GetTree().Paused = true;
        gameState.IsGamePaused = true;
	}

	public void Resume()
	{
        //Engine.TimeScale = 1;
        Hide();
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
    }

    public async void DelayResume()
    {
        Hide();
        await ToSignal(GetTree().CreateTimer(resumeDelay), "timeout");
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
    }
}
