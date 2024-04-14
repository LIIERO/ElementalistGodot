using System.Collections;
using System.Collections.Generic;
using Godot;

[System.Serializable]
public class InputPreferencesData
{
    public Dictionary<string, byte[]> Keybinds { get; private set; } = null;

    public InputPreferencesData(Dictionary<string, byte[]> Keybinds)
    {
        this.Keybinds = Keybinds;
    }
}
