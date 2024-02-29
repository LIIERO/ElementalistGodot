using Godot;
using GlobalTypes;

[GlobalClass]
public partial class WorldData : Resource
{
    [Export] public string ID { get; set; }
    [Export] public string Name { get; set; }
}