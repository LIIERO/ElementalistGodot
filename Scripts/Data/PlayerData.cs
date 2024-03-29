using System.Collections;
using System.Collections.Generic;
using Godot;

[System.Serializable]
public class PlayerData
{
    public Dictionary<string, Dictionary<string, bool>> CompletedLevels { get; private set; }
    public int NoSunFragments { get; private set; }
    public string CurrentWorld { get; private set; }
    public string PreviousWorld { get; private set; }
    public string CurrentLevel { get; private set; }
    public string PreviousLevel { get; private set; }

    public PlayerData(Dictionary<string, Dictionary<string, bool>> CompletedLevels, int NoSunFragments, string CurrentWorld, string PreviousWorld, string CurrentLevel, string PreviousLevel)
    {
        this.CompletedLevels = CompletedLevels;
        this.NoSunFragments = NoSunFragments;
        this.CurrentWorld = CurrentWorld;
        this.PreviousWorld = PreviousWorld;
        this.CurrentLevel = CurrentLevel;
        this.PreviousLevel = PreviousLevel;
    }
}
