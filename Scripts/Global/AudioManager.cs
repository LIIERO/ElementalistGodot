using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
    readonly private Random random = new();

    private Dictionary<string, AudioStreamPlayer> worldMusicDictionary;
    private AudioStreamPlayer currentMusic;
    private bool fadingOut = false;
    private const float FADETIME = 1.0f;
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
        if (!fadingOut) return;

        fadeTimeProgress -= (float)delta;
        float volume_db = GameUtils.LinearToDecibel(fadeTimeProgress / FADETIME); 
        AudioServer.SetBusVolumeDb(musicFadeBusId, volume_db);

        if (fadeTimeProgress < 0.0f)
        {
            fadingOut = false;
            currentMusic.Stop();
        }
    }

    public void PlayWorldMusic(string id)
    {
        fadingOut = false;
        AudioServer.SetBusVolumeDb(musicFadeBusId, 0f);
        currentMusic = worldMusicDictionary[id];
        currentMusic.Play();
    }

    public void StopMusic()
    {
        if (currentMusic == null) return;

        fadingOut = true;
        fadeTimeProgress = FADETIME;
        //currentMusic.Stop();
    }


    // Sound effects
    [ExportSubgroup("Player")]
    [Export] public AudioStreamPlayer[] softFootsteps;
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
