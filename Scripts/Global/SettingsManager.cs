using Godot;
using System;
using GlobalTypes;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;

public partial class SettingsManager : Node
{
    private GameState gameState;
    const int baseWindowWidth = 480;
    const int baseWindowHeight = 270;

    public bool Fullscreen { get; private set; } = true;
    public int WindowScale { get; private set; } = 2;
    public int SoundVolume { get; private set; } = 5;
    public int MusicVolume { get; private set; } = 5;
    public bool LightParticlesActive {  get; private set; } = true;
    public bool SpeedrunTimerVisible { get; private set; } = true;
    

    // audio volume stuff
    private const string soundBusName = "Sounds";
    private int soundBusId;
    private const string musicBusName = "Music";
    private int musicBusId;


    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true); // Window resize disabled (in code to fix a bug)
        soundBusId = AudioServer.GetBusIndex(soundBusName);
        musicBusId = AudioServer.GetBusIndex(musicBusName);

        LoadPreferences();
        LoadKeyboardKeybinds();
        LoadGamepadKeybinds();

        ChangeSoundVolume(SoundVolume);
        ChangeMusicVolume(MusicVolume);

        if (Fullscreen)
        {
            ChangeToFullscreen();
        } 
        else
        {
            ChangeWindowScale(WindowScale);
            ChangeToWindowed();
        }   
    }


    private void ChangeResolution(int windowScale = 1)
    {
        GetWindow().Size = new Vector2I(baseWindowWidth, baseWindowHeight) * windowScale;
        GetWindow().Size = new Vector2I(baseWindowWidth, baseWindowHeight) * windowScale; // bugfix
    }

    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusIn) // bugfix
        {
            if (!Fullscreen) ChangeResolution(WindowScale);
        }

        if (what == NotificationWMCloseRequest)
        {
            SavePreferences();
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

    public void SetLightAndParticlesVisibility(bool visible)
    {
        LightParticlesActive = visible;
    }

    public void SetTimerVisibility(bool active)
    {
        SpeedrunTimerVisible = active;
    }

    private const string preferencesPath = "user://preferences.json";
    public void SavePreferences()
    {
        string path = ProjectSettings.GlobalizePath(preferencesPath);
        PreferencesData data = new(Fullscreen, WindowScale, SoundVolume, MusicVolume, LightParticlesActive, SpeedrunTimerVisible);
        string jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(path, jsonString);
    }

    private void LoadPreferences()
    {
        string path = ProjectSettings.GlobalizePath(preferencesPath);
        if (!File.Exists(path))
        {
            GD.Print("Path not found (preferences)");
            //gameState.FirstBoot = true;
            return;
        }
        string jsonString = File.ReadAllText(path);
        PreferencesData data = JsonSerializer.Deserialize<PreferencesData>(jsonString)!;

        Fullscreen = data.Fullscreen;
        WindowScale = data.WindowScale;
        SoundVolume = data.SoundVolume;
        MusicVolume = data.MusicVolume;
        LightParticlesActive = data.LightParticlesActive;
        SpeedrunTimerVisible = data.SpeedrunTimerVisible;
    }

    private const string inputPreferencesPath = "user://inputPreferences.json";
    private const string inputPreferencesGamepadPath = "user://inputPreferencesGamepad.json";
    public void LoadKeyboardKeybinds()
    {
        string path = ProjectSettings.GlobalizePath(inputPreferencesPath);
        if (!File.Exists(path))
        {
            GD.Print("Path not found (input preferences)");
            return;
        }
        string jsonString = File.ReadAllText(path);
        InputPreferencesData data = JsonSerializer.Deserialize<InputPreferencesData>(jsonString)!;

        if (data.Keybinds == null) return;

        foreach (KeyValuePair<string, byte[]> actionEventPair in data.Keybinds)
        {
            InputMap.ActionEraseEvents(actionEventPair.Key);
            InputMap.ActionAddEvent(actionEventPair.Key, (InputEventKey)GD.BytesToVarWithObjects(actionEventPair.Value));
        }  
    }

    public void LoadGamepadKeybinds()
    {
        string pathGamepad = ProjectSettings.GlobalizePath(inputPreferencesGamepadPath);
        if (!File.Exists(pathGamepad))
        {
            GD.Print("Path not found (input preferences gamepad)");
            return;
        }
        string jsonStringGamepad = File.ReadAllText(pathGamepad);
        InputPreferencesData dataGamepad = JsonSerializer.Deserialize<InputPreferencesData>(jsonStringGamepad)!;

        if (dataGamepad.Keybinds == null) return;

        foreach (KeyValuePair<string, byte[]> actionEventPair in dataGamepad.Keybinds)
        {
            InputMap.ActionEraseEvents(actionEventPair.Key);
            InputMap.ActionAddEvent(actionEventPair.Key, (InputEventJoypadButton)GD.BytesToVarWithObjects(actionEventPair.Value));
        }
    }

    public void SaveKeybinds(Dictionary<string, byte[]> keybinds, bool gamepad = false)
    {
        string path = gamepad ? inputPreferencesGamepadPath : inputPreferencesPath;
        string pathG = ProjectSettings.GlobalizePath(path);
        InputPreferencesData data = new(keybinds);
        string jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(pathG, jsonString);
    }
}
