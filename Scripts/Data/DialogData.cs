using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DialogData : Resource
{
	[Export] public Dictionary<string, string[]> data;
}
