using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using GlobalTypes;
using System.Linq;
using System.IO;
using System.Reflection.Emit;

public partial class GameState : Node
{
    // Level loader stuff
    private readonly string levelsPathStart = "res://Scenes/Worlds/";
    private readonly Dictionary<string, string[]> levels = new() { // Turn this to json?
        { "H", new string[] { "HUB", "0" } }, // Main Hub (The Void)
        { "0", new string[] { "HUB", "0", "1", "2", "3", "4", "5" } }, // Purple Forest
        { "1", new string[] { "HUB", "0", "1", "2", "3", "4", "5", "6", "7", "A", "B", "C", "D", "E", "4S", "7S" } }, // Distant Shores
        { "2", new string[] { "HUB", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "4S", "8S" } }, // Cave Outskirts
        { "3", new string[] { "HUB", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "2S", "6S" } } // Islands of Ashes
    };

    private Dictionary<string, Dictionary<string, PackedScene>> LevelIDToLevel = new(); // Level path data, initialized in _Ready


    // Data loaded from the save file
    public Dictionary<string, Dictionary<string, bool>> CompletedLevels { get; private set; } = new(); // Initialized in _Ready if first game launch, or from save
    public int NoSunFragments { get; private set; } = 0;
    public int NoRedFragments { get; private set; } = 0;
    public string CurrentWorld { get; private set; } = "0";
    public string PreviousWorld { get; private set; } = "0";
    public string CurrentLevel { get; private set; } = "0"; // Current level ID
    public string PreviousLevel { get; private set; } = "0";
    public bool IsCurrentLevelSpecial { get; set; } = false;
    public string CurrentLevelName { get; set; } = "";


    // Data not loaded from the save file
    public Vector2 PlayerHubRespawnPosition { get; set; } = Vector2.Inf; // Hubs have multiple respawn points, Inf means base position in engine will be used
    public bool IsGameplayActive { get; private set; } = false; // Is the root of menus the main menu or the gameplay
    public string CurrentSaveFileID { get; private set; } = "0";
    public bool IsGamePaused { get; set; } = false; // Pause is set in pause menu
    public bool IsLevelTransitionPlaying { get; set; } = false;
    //public bool FirstBoot { get; set; } = false; // Set to true in SettingManager when creating preferences file
    

    // METHODS ===========================================================================================================
    private CustomSignals customSignals; // singleton
    public override void _EnterTree()
    {
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        // Initialize LevelIDToLevel
        foreach (KeyValuePair<string, string[]> world in levels)
        {
            LevelIDToLevel.Add(world.Key, new Dictionary<string, PackedScene>());

            foreach (string levelID in world.Value)
            {
                LevelIDToLevel[world.Key].Add(levelID, ResourceLoader.Load<PackedScene>(levelsPathStart + world.Key + "/" + levelID + ".tscn"));
            }
        }

        CompletedLevels = CreateNewCompletedLevelsDict(); // Initialize CompletedLevels

        // Initialize previous world, TODO: do this in save file
        PreviousWorld = CurrentWorld;
    }


    private Dictionary<string, Dictionary<string, bool>> CreateNewCompletedLevelsDict()
    {
        Dictionary<string, Dictionary<string, bool>> completedLevels = new();

        foreach (KeyValuePair<string, string[]> world in levels)
        {
            completedLevels.Add(world.Key, new Dictionary<string, bool>());

            foreach (string levelID in world.Value)
            {
                if (levelID != "HUB")
                    completedLevels[world.Key].Add(levelID, false);
            }
        }
        return completedLevels;
    }

    private void FixCompletedLevels()
    {
        foreach (KeyValuePair<string, string[]> world in levels)
        {
            if (!CompletedLevels.ContainsKey(world.Key))
            {
                CompletedLevels.Add(world.Key, new Dictionary<string, bool>());
            }

            foreach (string levelID in world.Value)
            {
                if (levelID != "HUB")
                {
                    if (!CompletedLevels[world.Key].ContainsKey(levelID))
                    {
                        CompletedLevels[world.Key].Add(levelID, false);
                    }
                }  
            }
        }
    }

    public void ResetPersistentData()
    {
        PlayerHubRespawnPosition = Vector2.Inf; // Hubs have multiple respawn points, Inf means base position in engine will be used
        IsGameplayActive = false; // Is the root of menus the main menu or the gameplay
        CurrentSaveFileID = "0";
        IsGamePaused = false; // Pause is set in pause menu
        IsLevelTransitionPlaying = false;
    }

    public void CompleteCurrentLevel()
    {
        if (HasCurrentLevelBeenCompleted()) return;

        if (IsCurrentLevelSpecial) NoRedFragments++;
        else NoSunFragments++;

        CompletedLevels[CurrentWorld][CurrentLevel] = true;
    }

    public bool HasWorldBeenCompleted(string world) { return !CompletedLevels[world].Values.Any(l => l == false); }
    public bool HasLevelBeenCompleted(string levelID)
    {
        if (levelID == "HUB") return true;
        return CompletedLevels[CurrentWorld][levelID] == true;
    }
    public bool HasCurrentLevelBeenCompleted() { return HasLevelBeenCompleted(CurrentLevel); }
    public bool IsHubLoaded() { return CurrentLevel == "HUB"; }

    public int GetNoLocalCompletedStandardLevels()
    {
        int noCompleted = 0;
        foreach (KeyValuePair<string, bool> level in CompletedLevels[CurrentWorld])
        {
            if (level.Value && !GameUtils.LevelIDEndsWithLetter(level.Key, "S")) noCompleted++;
        }
        return noCompleted;
    }

    public void LoadGame()
    {
        IsGameplayActive = true;
        GetTree().ChangeSceneToPacked(LevelIDToLevel[CurrentWorld][CurrentLevel]);
    }

    public void LoadWorld(string world)
    {
        IsCurrentLevelSpecial = false;
        PreviousWorld = CurrentWorld;
        CurrentWorld = world;
        LoadLevel("HUB");
        // World enter player position set in WorldEntrance because it was easier that way
        WorldEntrance.setPlayerWorldEnterPosition = true;
    }

    public void LoadLevel(string id, string name="HUB")
    {
        PreviousLevel = CurrentLevel;
        CurrentLevel = id;
        CurrentLevelName = name;
        GD.Print(CurrentLevelName);
        LoadGame();
    }

    public void RestartCurrentLevel()
    {
        GetTree().ReloadCurrentScene();
    }

    public void LoadHubLevel()
    {
        IsCurrentLevelSpecial = false;
        LoadLevel("HUB");
        // Level enter player position set in LevelTeleport because it was easier that way
        LevelTeleport.setPlayerLevelEnterPosition = true;
    }

    public void LoadMenu()
    {
        IsGameplayActive = false;
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn"));
    }

    public void LoadOptions()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/Options.tscn"));
    }

    public void LoadInputOptions()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/InputOptions.tscn"));
    }

    public void SetPlayerPosition(Vector2 position)
    {
        customSignals.EmitSignal(CustomSignals.SignalName.SetPlayerPosition, position);
    }


    // SAVE LOAD
    private const string savesPath = "user://save";
    private const string savesFormat = ".json";
    public void SaveToSaveFile(string id)
    {
        string path = ProjectSettings.GlobalizePath(savesPath + id + savesFormat);
        PlayerData data = new(CompletedLevels, NoSunFragments, NoRedFragments, CurrentWorld, PreviousWorld, CurrentLevel, PreviousLevel, IsCurrentLevelSpecial, CurrentLevelName);
        string jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(path, jsonString);
    }

    public void LoadFromSaveFile(string id)
    {
        string path = ProjectSettings.GlobalizePath(savesPath + id + savesFormat);
        if (!File.Exists(path))
        {
            GD.Print("Path not found");
            return;
        }
        string jsonString = File.ReadAllText(path);
        PlayerData data = JsonSerializer.Deserialize<PlayerData>(jsonString)!;

        CompletedLevels = data.CompletedLevels;
        NoSunFragments = data.NoSunFragments;
        NoRedFragments = data.NoRedFragments;
        CurrentWorld = data.CurrentWorld;
        PreviousWorld = data.PreviousWorld;
        CurrentLevel = data.CurrentLevel;
        PreviousLevel = data.PreviousLevel;
        IsCurrentLevelSpecial = data.IsCurrentLevelSpecial;
        CurrentLevelName = data.CurrentLevelName;

        FixCompletedLevels();

        CurrentSaveFileID = id;
    }

    public void CreateNewSaveFile(string id)
    {
        CompletedLevels = CreateNewCompletedLevelsDict();
        NoSunFragments = 0;
        NoRedFragments = 0;
        CurrentLevel = "HUB";
        PreviousLevel = "HUB";
        CurrentWorld = "0";
        PreviousWorld = "0";
        IsCurrentLevelSpecial = false;
        CurrentLevelName = "HUB";

        CurrentSaveFileID = id;

        SaveToSaveFile(id);
    }

    public bool SaveFileExists(string id)
    {
        string path = ProjectSettings.GlobalizePath(savesPath + id + savesFormat);
        return File.Exists(path);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (IsGameplayActive)
                SaveToSaveFile(CurrentSaveFileID);
        }
    }


    // DEBUG
    //private bool isGameDebugUnlocked = false;
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("inputDebugUnlockSpecific"))
        {
            if (IsHubLoaded()) // Unlock whole current world
            {
                foreach (string levelKey in CompletedLevels[CurrentWorld].Keys.ToList())
                {
                    if (CompletedLevels[CurrentWorld][levelKey] == false)
                    {
                        CompletedLevels[CurrentWorld][levelKey] = true;
                        if (GameUtils.LevelIDEndsWithLetter(levelKey, "S"))
                            NoRedFragments += 1;
                        else
                            NoSunFragments += 1;
                    }
                }
            }

            else // Unlock current level
            {
                CompleteCurrentLevel();
            }

            //isGameDebugUnlocked = true;
            RestartCurrentLevel();
        }

        else if (Input.IsActionJustPressed("inputDebugUnlockAll"))
        {
            NoSunFragments = 999;
            NoRedFragments = 99;
            foreach (KeyValuePair<string, Dictionary<string, bool>> world in CompletedLevels)
            {
                foreach (string levelKey in world.Value.Keys.ToList())
                {
                    CompletedLevels[world.Key][levelKey] = true;
                }
            }
            RestartCurrentLevel();
        }
    }
}

