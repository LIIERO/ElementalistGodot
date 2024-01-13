using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{
    // Singletons
    private CustomSignals customSignals;
    

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
                Resume();
            }
            if (CurrentButtonIndex == 1) // main menu
            {
                // TODO save progress
                // TODO switch to menu
            }
            if (CurrentButtonIndex == 2) // exit
            {
                // TODO save progress
                GetTree().Quit();
            }
        }
    }


	void _OnContinueButtonPressed()
	{
		Resume();
	}


    public void Pause()
	{
		//Engine.TimeScale = 0;
		GetTree().Paused = true;
		Show();
        gameState.IsGamePaused = true;
	}

	public void Resume()
	{
        //Engine.TimeScale = 1;
        GetTree().Paused = false;
        Hide();
        gameState.IsGamePaused = false;
    }
}
