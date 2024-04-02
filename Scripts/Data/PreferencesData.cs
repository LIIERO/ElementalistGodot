using System.Collections;
using System.Collections.Generic;
using Godot;

[System.Serializable]
public class PreferencesData
{
    public bool Fullscreen { get; private set; }
    public int WindowScale { get; private set; }
    public int SoundVolume { get; private set; }
    public int MusicVolume { get; private set; }

    public PreferencesData(bool Fullscreen, int WindowScale, int SoundVolume, int MusicVolume)
    {
        this.Fullscreen = Fullscreen;
        this.WindowScale = WindowScale;
        this.SoundVolume = SoundVolume;
        this.MusicVolume = MusicVolume;
    }
}
