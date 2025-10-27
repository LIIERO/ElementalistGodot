using System.Collections;
using System.Collections.Generic;
using GlobalTypes;
using Godot;

[System.Serializable]
public class PlayerData
{
    public float PlayerHubRespawnX { get; private set; } = 0f;
    public float PlayerHubRespawnY { get; private set; } = 0f;
    public Dictionary<string, Dictionary<string, bool>> CompletedLevels { get; private set; }
    public int NoSunFragments { get; private set; } = 0;
    public int NoRedFragments { get; private set; } = 0;
    public string CurrentWorld { get; private set; } = "0";
    public string PreviousWorld { get; private set; } = "0";
    public string CurrentLevel { get; private set; } = "HUB";
    public string PreviousLevel { get; private set; } = "HUB";
    public bool IsCurrentLevelSpecial { get; private set; } = false;
    public string CurrentLevelNameID { get; private set; } = "HUB";
    public int MainCutsceneProgress { get; private set; } = 0;
    public double InGameTime { get; private set; } = 0.0;
    public int NoDeaths { get; private set; } = 0;
    public int NoRestarts { get; private set; } = 0;
    public int NoUndos { get; private set; } = 0;
    public int NoAbilityUses { get; private set; } = 0;
    public bool IsAbilitySalvagingUnlocked { get; private set; } = false;
    public List<int> SalvagedAbilities { get; private set; } = new();
    public List<string> UnlockedLetters { get; set; } = new();
    public Dictionary<string, List<int>> AbilitiesSalvagedInLevels { get; private set; } = new();

    public PlayerData(
        float PlayerHubRespawnX, float PlayerHubRespawnY,
        Dictionary<string, Dictionary<string, bool>> CompletedLevels, 
        int NoSunFragments, 
        int NoRedFragments, 
        string CurrentWorld, 
        string PreviousWorld, 
        string CurrentLevel, 
        string PreviousLevel, 
        bool IsCurrentLevelSpecial, 
        string CurrentLevelNameID,
        int MainCutsceneProgress,
        double InGameTime,
        int NoDeaths,
        int NoRestarts,
        int NoUndos,
        int NoAbilityUses,
        bool IsAbilitySalvagingUnlocked,
        List<int> SalvagedAbilities,
        List<string> UnlockedLetters,
        Dictionary<string, List<int>> AbilitiesSalvagedInLevels)
    {
        this.PlayerHubRespawnX = PlayerHubRespawnX; this.PlayerHubRespawnY = PlayerHubRespawnY;
        this.CompletedLevels = CompletedLevels;
        this.NoSunFragments = NoSunFragments;
        this.NoRedFragments = NoRedFragments;
        this.CurrentWorld = CurrentWorld;
        this.PreviousWorld = PreviousWorld;
        this.CurrentLevel = CurrentLevel;
        this.PreviousLevel = PreviousLevel;
        this.IsCurrentLevelSpecial = IsCurrentLevelSpecial;
        this.CurrentLevelNameID = CurrentLevelNameID;
        this.MainCutsceneProgress = MainCutsceneProgress;
        this.InGameTime = InGameTime;
        this.NoDeaths = NoDeaths;
        this.NoRestarts = NoRestarts;
        this.NoUndos = NoUndos;
        this.NoAbilityUses = NoAbilityUses;
        this.IsAbilitySalvagingUnlocked = IsAbilitySalvagingUnlocked;
        this.SalvagedAbilities = SalvagedAbilities;
        this.UnlockedLetters = UnlockedLetters;
        this.AbilitiesSalvagedInLevels = AbilitiesSalvagedInLevels;
    }
}
