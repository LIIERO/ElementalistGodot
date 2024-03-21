using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class MainMenu : ButtonManager
{
    public static int sceneEnterItemIndex = 0; // which button is selected upon entering menu, set before switching to menu

    // Singletons
    private LevelTransitions levelTransitions;
    //private CustomSignals customSignals;

    public override void _Ready()
	{
        //GD.Print(sceneEnterItemIndex);
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;

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
                gameState.SaveToSaveFile("0"); // Create a new save with the default values
                levelTransitions.StartGameTransition(); // transition from menu to game
            }
            if (CurrentItemIndex == 1) // continue
            {
                gameState.LoadFromSaveFile("0");
                levelTransitions.StartGameTransition(); // transition from menu to game
            }
            if (CurrentItemIndex == 2) // options
            {
                levelTransitions.StartOptionsTransition();
            }
            if (CurrentItemIndex == 3) // exit
            {
                GetTree().Quit();
            }
        }
    }
}
