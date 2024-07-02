using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{
    // singletons
    //private SettingsManager settingsManager;
    private LevelTransitions levelTransitions;
    private AudioManager audioManager;
    private CustomSignals customSignals;

    const float resumeDelay = 0.1f;

	public override void _Ready()
	{
        //settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        base._Ready();
        Resume();
	}

    public override void _Process(double delta)
	{
        if (InputManager.PausePressed())
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

        if (InputManager.UIAcceptPressed())
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
        customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, true);
    }

	private void Resume()
	{
        //Engine.TimeScale = 1;
        Hide();

        GetTree().Paused = false;
        gameState.IsGamePaused = false;
        customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, false);
    }

    public async void DelayResume()
    {
        await ToSignal(GetTree().CreateTimer(resumeDelay), "timeout");
        Resume();
    }
}
