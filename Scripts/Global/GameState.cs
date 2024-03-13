using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;
using System.Linq;

public partial class GameState : Node
{
    // Level loader stuff
    private readonly string levelsPathStart = "res://Scenes/Worlds/";
    private readonly Dictionary<string, string[]> levels = new() { // Turn this to json?
        { "H", new string[] { "HUB" } }, // Main Hub (RGB)
        { "0", new string[] { "HUB", "0", "1", "2", "3", "4", "5" } }, // Purple Forest
        { "1", new string[] { "HUB", "0", "1", "2", "3", "4", "5", "6", "7", "A", "B" } }, // Distant Shores
        { "2", new string[] { "HUB", "0", "1", "A" } } // Cave Outskirts
    };

    private Dictionary<string, Dictionary<string, PackedScene>> LevelIDToLevel = new(); // Level path data, initialized in _Ready


    // Data loaded from the save file
    public Dictionary<string, Dictionary<string, bool>> CompletedLevels { get; private set; } = new(); // Initialized in _Ready if first game launch, or from save

    public string CurrentWorld { get; private set; } = "0";
    public string PreviousWorld { get; private set; } = "0";
    public string CurrentLevel { get; private set; } = "HUB"; // Current level ID
    public string PreviousLevel { get; private set; } = "HUB";


    // Data not loaded from the save file
    public int NoCompletedLevels { get; private set; } = 0;
    public Vector2 PlayerHubRespawnPosition { get; set; } = Vector2.Inf; // Hubs have multiple respawn points, Inf means base position in engine will be used
    public bool IsGamePaused { get; set; } = false; // Pause is set in pause menu
    public bool IsLevelTransitionPlaying { get; set; } = false;
    


    // METHODS ===========================================================================================================
    private CustomSignals customSignals; // singleton
    public override void _EnterTree()
    {
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        // Initialize LevelIDToLevel and CompletedLevels
        foreach (KeyValuePair<string, string[]> world in levels)
        {
            LevelIDToLevel.Add(world.Key, new Dictionary<string, PackedScene>());
            CompletedLevels.Add(world.Key, new Dictionary<string, bool>());

            foreach (string levelID in world.Value)
            {
                LevelIDToLevel[world.Key].Add(levelID, ResourceLoader.Load<PackedScene>(levelsPathStart + world.Key + "/" + levelID + ".tscn"));
                if (levelID != "HUB")
                    CompletedLevels[world.Key].Add(levelID, false);
            }
        }

        // Initialize previous world, TODO: do this in save file
        PreviousWorld = CurrentWorld;
    }

    public void CompleteCurrentLevel()
    {
        if (HasCurrentLevelBeenCompleted()) return;
        NoCompletedLevels++;
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

    public int GetNoLocalCompletedLevels()
    {
        int noCompleted = 0;
        foreach (bool isCompleted in CompletedLevels[CurrentWorld].Values)
        {
            if (isCompleted) noCompleted++;
        }
        return noCompleted;
    }

    public void LoadGame()
    {
        GetTree().ChangeSceneToPacked(LevelIDToLevel[CurrentWorld][CurrentLevel]);
    }

    public void LoadWorld(string world)
    {
        PreviousWorld = CurrentWorld;
        CurrentWorld = world;
        LoadLevel("HUB");
        // World enter player position set in WorldEntrance because it was easier that way
        WorldEntrance.setPlayerWorldEnterPosition = true;
    }

    public void LoadLevel(string id)
    {
        PreviousLevel = CurrentLevel;
        CurrentLevel = id;
        LoadGame();
    }

    public void RestartCurrentLevel()
    {
        GetTree().ReloadCurrentScene();
    }

    public void LoadHubLevel()
    {
        LoadLevel("HUB");
        // Level enter player position set in LevelTeleport because it was easier that way
        LevelTeleport.setPlayerLevelEnterPosition = true;
    }

    public void LoadMenu()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn"));
    }

    public void LoadOptions()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/Options.tscn"));
    }

    public void SetPlayerPosition(Vector2 position)
    {
        customSignals.EmitSignal(CustomSignals.SignalName.SetPlayerPosition, position);
    }


    // DEBUG
    private bool isGameDebugUnlocked = false;
    public override void _Process(double delta)
    {
        // Unlock everything to test stuff
        if (Input.IsActionJustPressed("inputDebugUnlockAll") && !isGameDebugUnlocked)
        {
            NoCompletedLevels = 999;
            foreach(KeyValuePair<string, Dictionary<string, bool>> world in CompletedLevels)
            {
                foreach(string levelKey in world.Value.Keys.ToList())
                {
                    CompletedLevels[world.Key][levelKey] = true;
                }
            }
            isGameDebugUnlocked = true;
            RestartCurrentLevel();
        }
        
    }
}
