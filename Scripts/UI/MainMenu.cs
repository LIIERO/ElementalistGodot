using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class MainMenu : ButtonManager
{
    public static int sceneEnterItemIndex = 0; // which button is selected upon entering menu, set before switching to menu

    // Singletons
    //private CustomSignals customSignals;

    public override void _Ready()
	{
        //GD.Print(sceneEnterItemIndex);
        startingIndex = sceneEnterItemIndex;
		base._Ready();
	}

    public override void _Process(double delta)
	{
		base._Process(delta);

        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == 0) // new game
            {
                // TODO: New game save resource
                gameState.LoadWorld(WorldID.PurpleForest); // TEMPORARY
            }
            if (CurrentItemIndex == 1) // continue
            {
                // TODO: Load game save resource
            }
            if (CurrentItemIndex == 2) // options
            {
                gameState.LoadOptions();
            }
            if (CurrentItemIndex == 3) // exit
            {
                GetTree().Quit();
            }
        }
    }
}
