using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{

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
			else if (!gameState.IsLevelTransitionPlaying) // Cant pause during transitions
            {
                ResetButtons();
                Pause();
            }
		}

        if (Input.IsActionJustPressed("ui_accept") && gameState.IsGamePaused)
        {
            if (CurrentItemIndex == 0) // resume
            {
                DelayResume();
            }
            if (CurrentItemIndex == 1) // main menu
            {
                Resume();
                // TODO save progress
                gameState.LoadMenu();
            }
            if (CurrentItemIndex == 2) // exit
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
