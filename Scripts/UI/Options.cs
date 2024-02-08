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
    private SettingsManager settingsManager;


    public override void _Ready()
	{
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        base._Ready();
        InitializeElementsEndFrame();
	}

    public override void _Process(double delta)
	{
		base._Process(delta);

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            gameState.LoadMenu();
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == MENU)
            {
                gameState.LoadMenu();
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
                resolutionSelection.MoveRight();
                if (resolutionSelection.Enabled) settingsManager.ChangeWindowScale(resolutionSelection.CurrentValueIndex + 1);
            }
        }

        if (Input.IsActionJustPressed("ui_left"))
        {
            if (CurrentItemIndex == RESOLUTION) // choose resolution
            {
                MenuSelection resolutionSelection = buttonList[CurrentItemIndex] as MenuSelection;
                resolutionSelection.MoveLeft();
                if (resolutionSelection.Enabled) settingsManager.ChangeWindowScale(resolutionSelection.CurrentValueIndex + 1);
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
