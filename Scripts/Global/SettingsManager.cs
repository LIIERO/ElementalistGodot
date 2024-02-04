using Godot;
using System;
using GlobalTypes;
using System.Collections.Generic;

public partial class SettingsManager : Node
{
    const int baseWindowWidth = 480;
    const int baseWindowHeight = 270;


    public bool Fullscreen { get; private set; }
    public int WindowScale { get; private set; } = 2; // Set with resolution option TODO


    public override void _Ready()
    {
        //Fullscreen = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
        ChangeToWindowed(); // Temporary, TODO: save file for player preferences that are loaded in this method
    }


    public void ChangeResolution(int windowScale = 1)
    {
        GetWindow().Size = new Vector2I(baseWindowWidth, baseWindowHeight) * windowScale;
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
        ChangeResolution(WindowScale);
    }
}
