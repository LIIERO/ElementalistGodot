using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;
using System.Linq;

public partial class GameState : Node
{
    // Level loader stuff
    private readonly string levelsPathStart = "res://Scenes/Worlds/";
    private readonly Dictionary<WorldID, string[]> levels = new() { // Turn this to json?
        { WorldID.PurpleForest, new string[] { "HUB", "0", "1", "2", "3", "4", "5" } },
        { WorldID.DistantShoreline, new string[] { "HUB", "0" } } 
    };

    /*private readonly Dictionary<string, string> worldToWorldName = new() {
        { "PurpleForest", "Purple Forest" }
    };*/

    private Dictionary<WorldID, Dictionary<string, PackedScene>> LevelIDToLevel = new(); // Level path data, initialized in _Ready


    // Data loaded from the save file
    public Dictionary<WorldID, Dictionary<string, bool>> CompletedLevels { get; private set; } = new(); // Initialized in _Ready if first game launch, or from save

    public WorldID CurrentWorld { get; private set; } = WorldID.PurpleForest;
    public string CurrentLevel { get; private set; } = "0"; // Current level ID
    public Vector2 PlayerHubPosition { get; set; } = Vector2.Zero; // Updated in LevelTeleport


    // Data not loaded from the save file
    public int NoCompletedLevels { get; private set; } = 0;
    public bool IsGamePaused { get; set; } = false; // Pause is set in pause menu
    public bool IsLevelTransitionPlaying { get; set; } = false;
    public WorldID PreviousWorld { get; private set; }


    // METHODS ===========================================================================================================
    private CustomSignals customSignals; // singleton
    public override void _EnterTree()
    {
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        // Initialize LevelIDToLevel and CompletedLevels
        foreach (KeyValuePair<WorldID, string[]> world in levels)
        {
            LevelIDToLevel.Add(world.Key, new Dictionary<string, PackedScene>());
            CompletedLevels.Add(world.Key, new Dictionary<string, bool>());

            foreach (string levelID in world.Value)
            {
                LevelIDToLevel[world.Key].Add(levelID, ResourceLoader.Load<PackedScene>(levelsPathStart + world.Key.ToString() + "/" + levelID + ".tscn"));
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

    public bool HasWorldBeenCompleted(WorldID world) { return !CompletedLevels[world].Values.Any(l => l == false); }
    public bool HasLevelBeenCompleted(string levelID) { return CompletedLevels[CurrentWorld][levelID] == true; }
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

    public void LoadWorld(WorldID world)
    {
        PreviousWorld = CurrentWorld;
        CurrentWorld = world;
        LoadLevel("HUB");
        // World enter player position set in WorldEntrance because it was easier that way
        WorldEntrance.setPlayerWorldEnterPosition = true;
    }

    public void LoadLevel(string id)
    {
        CurrentLevel = id;
        GetTree().ChangeSceneToPacked(LevelIDToLevel[CurrentWorld][CurrentLevel]);
    }

    public void RestartCurrentLevel()
    {
        if (CurrentLevel != "HUB") GetTree().ReloadCurrentScene();
    }

    public void LoadHubLevel()
    {
        LoadLevel("HUB");
        SetPlayerPosition(PlayerHubPosition);
    }

    public async void SetPlayerPosition(Vector2 position)
    {
        if (position == Vector2.Zero) return;
        await ToSignal(GetTree(), "process_frame");
        customSignals.EmitSignal(CustomSignals.SignalName.SetPlayerPosition, position);
        customSignals.EmitSignal(CustomSignals.SignalName.SetCameraPosition, position);
    }

    public void LoadMenu()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn"));
    }
}
