using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;

public partial class GameState : Node
{
    // Data loaded from the save file
    public Dictionary<World, List<string>> CompletedLevels { get; private set; } = new()
    { { World.PurpleForest, new List<string>() }, { World.DistantShoreline, new List<string>() } };

    public World CurrentWorld { get; private set; } = World.PurpleForest;
    public string CurrentLevel { get; private set; } = "Sample Level";


    // Data not loaded from the save file
    public int NoCompletedLevels { get; private set; } = 0;
    public bool IsGamePaused { get; set; } = false; // Pause is set in pause menu


    // Methods
    public void CompleteLevel(string levelName)
    {
        NoCompletedLevels++;
        CompletedLevels[CurrentWorld].Add(levelName);
    }

    public bool HasLevelBeenCompleted(string levelName)
    {
        return CompletedLevels[CurrentWorld].Contains(levelName);
    }
}
