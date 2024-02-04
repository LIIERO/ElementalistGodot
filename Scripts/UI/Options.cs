using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Options : ButtonManager
{
    // Singletons
    private SettingsManager settingsManager;

    

    public override void _Ready()
	{
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        base._Ready();
        ToggleCheckboxesEndFrame();
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
            if (CurrentItemIndex == 0) // go back to main menu
            {
                gameState.LoadMenu();
            }
            if (CurrentItemIndex == 1) // toggle fullscreen
            {
                if (settingsManager.Fullscreen)
                {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(false);
                    settingsManager.ChangeToWindowed();
                }
                else
                {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(true);
                    settingsManager.ChangeToFullscreen();
                }
            }
            if (CurrentItemIndex == 2) // choose resolution
            {
                // TODO
            }
        }
    }

    async void ToggleCheckboxesEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");

        (buttonList[1] as MenuToggle).Toggle(settingsManager.Fullscreen);
    }

    /*private void SwapResolution()
    {
        if (Input.IsActionJustPressed("inputLeft"))
        {
            settingsManager.WindowScale -= 1;
            if (settingsManager.WindowScale < 1) settingsManager.WindowScale = 4;
            ChangeResolution(settingsManager.WindowScale);
        }
        else if (Input.IsActionJustPressed("inputRight"))
        {
            settingsManager.WindowScale += 1;
            if (settingsManager.WindowScale > 4) settingsManager.WindowScale = 1;
            ChangeResolution(settingsManager.WindowScale);
        }
    }*/
}
