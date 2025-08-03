using Godot;

[GlobalClass]
public partial class LevelData : Resource
{
	[Export] public string ID { get; set; }
    [Export] public string Name { get; set; }
    [Export] public int NoCompletedToUnlock { get; set; }
    [Export] public string SpecificLevelCompletedToUnlock { get; set; } = string.Empty;
    [Export] public bool IsSpecial { get; set; } = false;
    [Export] public bool IsSalvageable { get; set; } = false;
}
