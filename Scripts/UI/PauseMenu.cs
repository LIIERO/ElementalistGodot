using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{
    // singletons
    //private SettingsManager settingsManager;
    private LevelTransitions levelTransitions;
    private AudioManager audioManager;
    //private CustomSignals customSignals;

    const float resumeDelay = 0.1f;

	public override void _Ready()
	{
        //settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        base._Ready();
        Resume();
	}

    public override void _Process(double delta)
	{
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

        if (!gameState.IsGamePaused) return;

		base._Process(delta); // button stuff

        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == 0) // resume
            {
                DelayResume();
            }
            if (CurrentItemIndex == 1) // options
            {
                gameState.SaveToSaveFile(gameState.CurrentSaveFileID);
                audioManager.StopMusic();
                Resume();
                levelTransitions.StartOptionsTransition();
            }
            if (CurrentItemIndex == 2) // main menu (save and exit)
            {
                gameState.SaveToSaveFile(gameState.CurrentSaveFileID);
                audioManager.StopMusic();
                Resume();
                MainMenu.sceneEnterItemIndex = 1; // Menu starts with continue selected
                levelTransitions.StartMenuTransition();
            }
            /*if (CurrentItemIndex == 3) // exit game
            {
                audioManager.StopMusic();
                gameState.SaveToSaveFile("0");
                settingsManager.SavePreferences();
                GetTree().Quit();
            }*/
        }
    }

    private void Pause()
	{
        //Engine.TimeScale = 0;
        //customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, true);
        Show();
        GetTree().Paused = true;
        gameState.IsGamePaused = true;
    }

	private void Resume()
	{
        //Engine.TimeScale = 1;
        //customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, false);
        Hide();
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
        
    }

    public async void DelayResume()
    {
        Hide();
        await ToSignal(GetTree().CreateTimer(resumeDelay), "timeout");
        //customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, false);
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
    }
}
