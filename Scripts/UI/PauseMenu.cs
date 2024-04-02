using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{
    // singletons
    private SettingsManager settingsManager;
    private LevelTransitions levelTransitions;
    private AudioManager audioManager;

    const float resumeDelay = 0.1f;

	public override void _Ready()
	{
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

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
            if (CurrentItemIndex == 1) // main menu
            {
                audioManager.StopMusic();
                Resume();
                gameState.SaveToSaveFile("0");
                MainMenu.sceneEnterItemIndex = 1; // Menu starts with continue selected
                levelTransitions.StartMenuTransition();
            }
            if (CurrentItemIndex == 2) // exit
            {
                audioManager.StopMusic();
                gameState.SaveToSaveFile("0");
                settingsManager.SavePreferences();
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
