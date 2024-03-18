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

    public int SoundVolume { get; private set; } = 5;
    public int MusicVolume { get; private set; } = 5;


    // audio volume stuff
    private const string soundBusName = "Sounds";
    private int soundBusId;
    private const string musicBusName = "Music";
    private int musicBusId;


    public override void _Ready()
    {
        soundBusId = AudioServer.GetBusIndex(soundBusName);
        musicBusId = AudioServer.GetBusIndex(musicBusName);

        // TODO: Read settings save file and apply

        ChangeSoundVolume(SoundVolume);
        ChangeMusicVolume(MusicVolume);

        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true); // Window resize disabled (in code to fix a bug)
        Fullscreen = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
        if (Fullscreen) Input.MouseMode = Input.MouseModeEnum.Hidden;
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
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public void ChangeToWindowed()
    {
        Fullscreen = false;
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        RefreshResolutionEndFrame();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        //DisplayServer.WindowSetPosition(new Vector2I(baseWindowWidth, baseWindowHeight)); // Move window a bit away from the edge
    }

    async void RefreshResolutionEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");
        if (!Fullscreen) ChangeResolution(WindowScale);
    }

    public void ChangeSoundVolume(int volume) // volume from 0 to 10
    {
        SoundVolume = volume;
        float volume_db = GameUtils.LinearToDecibel(volume / 10f);
        AudioServer.SetBusVolumeDb(soundBusId, volume_db);
    }

    public void ChangeMusicVolume(int volume) // volume from 0 to 10
    {
        MusicVolume = volume;
        float volume_db = GameUtils.LinearToDecibel(volume / 10f);
        AudioServer.SetBusVolumeDb(musicBusId, volume_db);
    }
}
