using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Save : Resource
{
    public Dictionary<string, Dictionary<string, bool>> CompletedLevels { get; set; }
    public string CurrentWorld { get; set; }
    public string PreviousWorld { get; set; }
    public string CurrentLevel { get; set; }
    public string PreviousLevel { get; set; }
}
