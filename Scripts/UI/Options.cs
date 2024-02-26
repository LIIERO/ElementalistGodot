using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Options : ButtonManager
{
    // Button indexes
    private const int MENU = 0;
    private const int FULLSCREEN = 1;
    private const int RESOLUTION = 2;

    // Singletons
    private LevelTransitions levelTransitions;
    private SettingsManager settingsManager;


    public override void _Ready()
	{
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        base._Ready();
        InitializeElementsEndFrame();
	}

    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;

		base._Process(delta);

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            MainMenu.sceneEnterItemIndex = 2; // menu starts with options selected
            levelTransitions.StartMenuTransition();
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == MENU)
            {
                MainMenu.sceneEnterItemIndex = 2; // menu starts with options selected
                levelTransitions.StartMenuTransition();
            }
            if (CurrentItemIndex == FULLSCREEN) // toggle fullscreen
            {
                if (settingsManager.Fullscreen)
                {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(false);
                    (buttonList[RESOLUTION] as MenuSelection).Enable();
                    settingsManager.ChangeToWindowed();
                }
                else
                {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(true);
                    (buttonList[RESOLUTION] as MenuSelection).Disable();
                    settingsManager.ChangeToFullscreen();
                }
            }
            
        }


        // Move right or left for selections
        if (Input.IsActionJustPressed("ui_right"))
        {
            if (CurrentItemIndex == RESOLUTION) // choose resolution
            {
                MenuSelection resolutionSelection = buttonList[CurrentItemIndex] as MenuSelection;
                bool moved = resolutionSelection.MoveRight();
                if (moved) settingsManager.ChangeWindowScale(resolutionSelection.CurrentValueIndex + 1);
            }
        }

        if (Input.IsActionJustPressed("ui_left"))
        {
            if (CurrentItemIndex == RESOLUTION) // choose resolution
            {
                MenuSelection resolutionSelection = buttonList[CurrentItemIndex] as MenuSelection;
                bool moved = resolutionSelection.MoveLeft();
                if (moved) settingsManager.ChangeWindowScale(resolutionSelection.CurrentValueIndex + 1);
            }
        }
    }

    async void InitializeElementsEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");

        (buttonList[FULLSCREEN] as MenuToggle).Toggle(settingsManager.Fullscreen);
        if (settingsManager.Fullscreen) (buttonList[RESOLUTION] as MenuSelection).Disable();
        (buttonList[RESOLUTION] as MenuSelection).SetCurrentValueIndex(settingsManager.WindowScale - 1); // Set to current resoluton option
    }
}
