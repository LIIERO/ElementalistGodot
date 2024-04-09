using System.Collections;
using System.Collections.Generic;
using Godot;

[System.Serializable]
public class PreferencesData
{
    public bool Fullscreen { get; private set; } = true;
    public int WindowScale { get; private set; } = 2;
    public int SoundVolume { get; private set; } = 5;
    public int MusicVolume { get; private set; } = 5;
    public bool LightParticlesActive { get; private set; } = true;

    public PreferencesData(bool Fullscreen, int WindowScale, int SoundVolume, int MusicVolume, bool LightParticlesActive)
    {
        this.Fullscreen = Fullscreen;
        this.WindowScale = WindowScale;
        this.SoundVolume = SoundVolume;
        this.MusicVolume = MusicVolume;
        this.LightParticlesActive = LightParticlesActive;
    }
}
