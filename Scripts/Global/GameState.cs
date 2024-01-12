using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;

public partial class GameState : Node
{
    // Data loaded from the save file
    public Dictionary<World, List<string>> CompletedLevels { get; private set; } = new();



    // Data not loaded from the save file
    public int NoCompletedLevels { get; private set; } = 0;
    public bool IsGamePaused { get; set; } = false; // Pause is set in pause menu


    // Methods
    public void CompleteLevel(World world, string levelName)
    {
        NoCompletedLevels++;
        CompletedLevels[world].Add(levelName);
    }
}
