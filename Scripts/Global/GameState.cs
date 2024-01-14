using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;

public partial class GameState : Node
{
    // Level loader stuff
    private readonly string levelsPathStart = "res://Scenes/Worlds/";
    private readonly Dictionary<string, string[]> levels = new() { // Turn this to json?
        { "PurpleForest", new string[] { "HUB", "0" } },
        { "DistantShoreline", new string[] {} } 
    };

    private Dictionary<string, Dictionary<string, PackedScene>> LevelIDToLevel = new(); // Level path data, initialized in _Ready


    // Data loaded from the save file
    public Dictionary<string, List<string>> CompletedLevels { get; private set; } = new(); // Initialized in _Ready if first game launch, or from save

    public string CurrentWorld { get; private set; } = "PurpleForest";
    public string CurrentLevel { get; private set; } = ""; // ID of current gameplay level (hubs excluded)
    public Vector2 PlayerHubPosition { get; set; } // Updated in LevelTeleport


    // Data not loaded from the save file
    public int NoCompletedLevels { get; private set; } = 0;
    public bool IsGamePaused { get; set; } = false; // Pause is set in pause menu
    //public PackedScene currentHubScene { get; private set; }


    // METHODS ===========================================================================================================
    private CustomSignals customSignals;
    public override void _Ready()
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

        // Initialize CompletedLevels
        foreach (string worldName in levels.Keys)
        {
            CompletedLevels.Add(worldName, new List<string>());
        }
    }

    public void CompleteCurrentLevel()
    {
        if (HasCurrentLevelBeenCompleted()) return;
        NoCompletedLevels++;
        CompletedLevels[CurrentWorld].Add(CurrentLevel);
    }

    public bool HasCurrentLevelBeenCompleted()
    {
        return CompletedLevels[CurrentWorld].Contains(CurrentLevel);
    }

    public void LoadWorld(string worldName)
    {
        CurrentWorld = worldName;
        throw new NotImplementedException();
    }

    public void LoadLevel(string id)
    {
        CurrentLevel = id;
        GetTree().ChangeSceneToPacked(LevelIDToLevel[CurrentWorld][CurrentLevel]);
    }

    public void LoadHub()
    {
        GetTree().ChangeSceneToPacked(LevelIDToLevel[CurrentWorld]["HUB"]);
        SetPlayerHubPosition();
    }

    private async void SetPlayerHubPosition()
    {
        await ToSignal(GetTree(), "process_frame");
        customSignals.EmitSignal(CustomSignals.SignalName.SetPlayerPosition, PlayerHubPosition);
    }
}
