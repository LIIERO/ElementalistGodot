using Godot;
using GlobalTypes;
using Godot.Collections;

[GlobalClass]
public partial class LevelCollection : Resource
{
    [Export] public Dictionary<string, string[]> Levels { get; private set; }
}