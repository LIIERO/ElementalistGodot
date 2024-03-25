using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
    readonly private Random random = new();

    private Dictionary<string, AudioStreamPlayer> worldMusicDictionary;
    private AudioStreamPlayer currentMusic;
    private bool fadingOut = false;
    private bool fadingIn = false;
    private const float FADEOUTTIME = 1.0f;
    private const float FADEINTIME = 6.0f;
    private float fadeTimeProgress;

    private int musicFadeBusId;

    public override void _Ready()
    {
        musicFadeBusId = AudioServer.GetBusIndex("MusicFade");

        // Create music dictionary
        worldMusicDictionary = new Dictionary<string, AudioStreamPlayer>()
        {
            { "H", musicPurpleForest },
            { "0", musicPurpleForest },
            { "1", musicDistantShore },
            { "2", musicDistantShore }
        };
    }

    public override void _Process(double delta)
    {
        if (fadingOut) FadeOutProcess(delta);
        if (fadingIn) FadeInProcess(delta);
    }

    private void FadeOutProcess(double delta)
    {
        fadeTimeProgress -= (float)delta;
        if (fadeTimeProgress < 0.0f)
        {
            fadingOut = false;
            StopMusic();
        }

        float volume_db = GameUtils.LinearToDecibel(fadeTimeProgress / FADEOUTTIME);
        AudioServer.SetBusVolumeDb(musicFadeBusId, volume_db);

        
    }

    private void FadeInProcess(double delta)
    {
        fadeTimeProgress -= (float)delta;
        if (fadeTimeProgress < 0.0f)
        {
            fadingIn = false;
            fadeTimeProgress = 0.0f;
        }

        float volume_db = GameUtils.LinearToDecibel((FADEINTIME - fadeTimeProgress) / FADEINTIME);
        AudioServer.SetBusVolumeDb(musicFadeBusId, volume_db);
    }

    public void PlayWorldMusic(string worldId)
    {
        fadingIn = true;
        fadingOut = false;
        fadeTimeProgress = FADEINTIME;
        AudioServer.SetBusVolumeDb(musicFadeBusId, 0f);
        currentMusic = worldMusicDictionary[worldId];
        currentMusic.Play();
    }

    public void StopMusic()
    {
        if (currentMusic == null) return;
        fadingIn = false;
        currentMusic.Stop();
        currentMusic = null;
    }

    public void StopMusicWithFade()
    {
        if (currentMusic == null) return;
        fadingIn = false;
        fadingOut = true;
        fadeTimeProgress = FADEOUTTIME;
    }


    // Sound effects
    [ExportSubgroup("Player")]
    [Export] public AudioStreamPlayer[] softFootsteps;
    [Export] public AudioStreamPlayer[] hardFootsteps;
    [Export] public AudioStreamPlayer waterAbility;
    [Export] public AudioStreamPlayer airAbility;
    [Export] public AudioStreamPlayer earthAbilityStart;
    [Export] public AudioStreamPlayer earthAbilityEnd;

    [Export] public AudioStreamPlayer death;

    [ExportSubgroup("Entities")]
    [Export] public AudioStreamPlayer orbCollectSound;
    [Export] public AudioStreamPlayer sunCollectSound;
    [Export] public AudioStreamPlayer elementCharge;

    [ExportSubgroup("UI")]
    [Export] public AudioStreamPlayer buttonSelected;

    [ExportSubgroup("Transitions")]
    [Export] public AudioStreamPlayer levelCompleted;


    public void PlayRandomSound(AudioStreamPlayer[] soundPool)
    {
        int index = random.Next(soundPool.Length);
        soundPool[index].Play();
    }

    // Music
    [ExportSubgroup("Music")]
    [Export] public AudioStreamPlayer musicPurpleForest;
    [Export] public AudioStreamPlayer musicDistantShore;
}
