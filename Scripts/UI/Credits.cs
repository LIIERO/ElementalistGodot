using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Credits : ButtonManager
{

    // Singletons
    private LevelTransitions levelTransitions;

    public override void _Ready()
	{
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        base._Ready();
	}

    private void GoBack()
    {
        MainMenu.sceneEnterItemIndex = 4; // menu starts with options selected
        levelTransitions.StartMenuTransition();
    }

    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;

		base._Process(delta);

        if (InputManager.UICancelPressed())
        {
            GoBack();
        }

        else if (InputManager.UIAcceptPressed())
        {
            GoBack();  
        }
    }
}
