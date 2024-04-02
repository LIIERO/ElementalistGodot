using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class MainMenu : ButtonManager
{
    public static int sceneEnterItemIndex = 0; // which button is selected upon entering menu, set before switching to menu

    // Singletons
    private LevelTransitions levelTransitions;
    private SettingsManager settingsManager;
    //private CustomSignals customSignals;

    public override void _Ready()
	{
        //GD.Print(sceneEnterItemIndex);
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        gameState = GetNode<GameState>("/root/GameState");

        if (sceneEnterItemIndex == 0 && gameState.SaveFileExists("0")) // If there is a save file, the "continue" button will be selected first
            sceneEnterItemIndex = 1;

        startingIndex = sceneEnterItemIndex;
		base._Ready();
	}

    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;

        base._Process(delta);

        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == 0) // new game
            {
                gameState.CreateNewSaveFile("0"); // Create a new save with the default values
                levelTransitions.StartGameTransition(); // transition from menu to game
            }
            if (CurrentItemIndex == 1) // continue
            {
                if (!gameState.SaveFileExists("0")) return;
                gameState.LoadFromSaveFile("0");
                levelTransitions.StartGameTransition(); // transition from menu to game
            }
            if (CurrentItemIndex == 2) // options
            {
                levelTransitions.StartOptionsTransition();
            }
            if (CurrentItemIndex == 3) // exit
            {
                settingsManager.SavePreferences();
                GetTree().Quit();
            }
        }
    }
}
