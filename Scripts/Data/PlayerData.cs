using System.Collections;
using System.Collections.Generic;
using GlobalTypes;
using Godot;

[System.Serializable]
public class PlayerData
{
    public Dictionary<string, Dictionary<string, bool>> CompletedLevels { get; private set; }
    public int NoSunFragments { get; private set; } = 0;
    public int NoRedFragments { get; private set; } = 0;
    public string CurrentWorld { get; private set; } = "0";
    public string PreviousWorld { get; private set; } = "0";
    public string CurrentLevel { get; private set; } = "HUB";
    public string PreviousLevel { get; private set; } = "HUB";
    public bool IsCurrentLevelSpecial { get; private set; } = false;
    public string CurrentLevelName { get; private set; } = "HUB";
    public int MainCutsceneProgress { get; private set; } = 0;
    public double InGameTime { get; private set; } = 0.0;
    public int NoDeaths { get; private set; } = 0;
    public int NoRestarts { get; private set; } = 0;
    public int NoUndos { get; private set; } = 0;
    public bool IsAbilitySalvagingUnlocked { get; private set; } = false;
    public List<int> SalvagedAbilities { get; private set; } = new();

    public PlayerData(Dictionary<string, Dictionary<string, bool>> CompletedLevels, 
        int NoSunFragments, 
        int NoRedFragments, 
        string CurrentWorld, 
        string PreviousWorld, 
        string CurrentLevel, 
        string PreviousLevel, 
        bool IsCurrentLevelSpecial, 
        string CurrentLevelName,
        int MainCutsceneProgress,
        double InGameTime,
        int NoDeaths,
        int NoRestarts,
        int NoUndos,
        bool IsAbilitySalvagingUnlocked,
        List<int> SalvagedAbilities)
    {
        this.CompletedLevels = CompletedLevels;
        this.NoSunFragments = NoSunFragments;
        this.NoRedFragments = NoRedFragments;
        this.CurrentWorld = CurrentWorld;
        this.PreviousWorld = PreviousWorld;
        this.CurrentLevel = CurrentLevel;
        this.PreviousLevel = PreviousLevel;
        this.IsCurrentLevelSpecial = IsCurrentLevelSpecial;
        this.CurrentLevelName = CurrentLevelName;
        this.MainCutsceneProgress = MainCutsceneProgress;
        this.InGameTime = InGameTime;
        this.NoDeaths = NoDeaths;
        this.NoRestarts = NoRestarts;
        this.NoUndos = NoUndos;
        this.IsAbilitySalvagingUnlocked = IsAbilitySalvagingUnlocked;
        this.SalvagedAbilities = SalvagedAbilities;
    }
}
