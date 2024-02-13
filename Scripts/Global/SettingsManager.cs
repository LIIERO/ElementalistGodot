using Godot;
using System;
using GlobalTypes;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class SettingsManager : Node
{
    const int baseWindowWidth = 480;
    const int baseWindowHeight = 270;


    public bool Fullscreen { get; private set; }
    public int WindowScale { get; private set; } = 2; // Set with resolution option TODO


    public override void _Ready()
    {
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true); // Window resize disabled (in code to fix a bug)
        Fullscreen = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
        //ChangeToWindowed(); // Temporary, TODO: save file for player preferences that are loaded in this method
    }


    private void ChangeResolution(int windowScale = 1)
    {
        GetWindow().Size = new Vector2I(baseWindowWidth, baseWindowHeight) * windowScale;
        GetWindow().Size = new Vector2I(baseWindowWidth, baseWindowHeight) * windowScale; // bugfix
    }

    public override void _Notification(int what) // bugfix
    {
        if (what == NotificationApplicationFocusIn)
        {
            if (!Fullscreen) ChangeResolution(WindowScale);
        }
    }

    public void ChangeWindowScale(int windowScale)
    {
        WindowScale = windowScale;
        ChangeResolution(windowScale);
    }

    public void ChangeToFullscreen()
    {
        Fullscreen = true;
        ChangeResolution(); // The base window size to be scaled by exclusive fullscreen
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
    }

    public void ChangeToWindowed()
    {
        Fullscreen = false;
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        RefreshResolutionEndFrame();
        //DisplayServer.WindowSetPosition(new Vector2I(baseWindowWidth, baseWindowHeight)); // Move window a bit away from the edge
    }

    async void RefreshResolutionEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");
        if (!Fullscreen) ChangeResolution(WindowScale);
    }
}
