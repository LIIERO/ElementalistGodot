using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
    private GameState gameState;
    //private CustomSignals customSignals;

    readonly private Random random = new();

    private Dictionary<string, AudioStreamPlayer> worldMusicDictionary;
    private AudioStreamPlayer currentMusic;
    private bool fadingOut = false;
    private bool fadingIn = false;
    private const float FADEOUTTIME = 1.0f;
    private const float FADEINTIME = 6.0f;
    private float fadeTimeProgress;
    private float music_volume_db;

    private int musicFadeBusId;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        //customSignals.Connect(CustomSignals.SignalName.LevelTransitioned, new Callable(this, MethodName.PlayCurrentWorldMusicAfterFirstGoalGet));
        musicFadeBusId = AudioServer.GetBusIndex("MusicFade");

        // Create music dictionary
        worldMusicDictionary = new Dictionary<string, AudioStreamPlayer>()
        {
            { "H", musicVoid },
            { "0", musicPurpleForest },
            { "1", musicDistantShore },
            { "2", musicCaveOutskirts },
            { "3", musicIslandsOfAshes },
            { "4", musicVoid }
        };
    }

    public override void _Process(double delta)
    {
        if (fadingOut) FadeOutProcess(delta);
        else if (fadingIn) FadeInProcess(delta);
    }

    private void FadeOutProcess(double delta)
    {
        fadeTimeProgress -= (float)delta;
        if (fadeTimeProgress < 0.0f)
        {
            fadingOut = false;
            StopMusic();
        }

        float new_music_volume_db = GameUtils.LinearToDecibel(fadeTimeProgress / FADEOUTTIME);
        music_volume_db = new_music_volume_db <= music_volume_db ? new_music_volume_db : music_volume_db;
        AudioServer.SetBusVolumeDb(musicFadeBusId, music_volume_db);
    }

    private void FadeInProcess(double delta)
    {
        fadeTimeProgress -= (float)delta;
        if (fadeTimeProgress < 0.0f)
        {
            fadingIn = false;
            fadeTimeProgress = 0.0f;
        }

        music_volume_db = GameUtils.LinearToDecibel((FADEINTIME - fadeTimeProgress) / FADEINTIME);
        AudioServer.SetBusVolumeDb(musicFadeBusId, music_volume_db);
    }

    public void PlayWorldMusic(string worldId)
    {
        //if (gameState.MainCutsceneProgress == 0 && gameState.NoSunFragments == 0) return; // Dont play the music at the start

        fadingIn = true;
        fadingOut = false;
        fadeTimeProgress = FADEINTIME;
        AudioServer.SetBusVolumeDb(musicFadeBusId, 0f);
        currentMusic = worldMusicDictionary[worldId];
        currentMusic.Play();
    }

    /*public void PlayCurrentWorldMusicAfterFirstGoalGet() // Cutscene 0
    {
        if (gameState.MainCutsceneProgress == 0 && gameState.NoSunFragments > 0)
        {
            gameState.MainCutsceneProgress = 1;
            PlayWorldMusic(gameState.CurrentWorld);
        }
    }*/

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
    [Export] public AudioStreamPlayer fireAbility;
    [Export] public AudioStreamPlayer earthAbilityStart;
    [Export] public AudioStreamPlayer earthAbilityEnd;

    [Export] public AudioStreamPlayer death;

    [ExportSubgroup("Entities")]
    [Export] public AudioStreamPlayer orbCollectSound;
    [Export] public AudioStreamPlayer sunCollectSound;
    [Export] public AudioStreamPlayer elementCharge;
    [Export] public AudioStreamPlayer gateOpen;

    [ExportSubgroup("UI")]
    [Export] public AudioStreamPlayer buttonSelected;

    [ExportSubgroup("Transitions")]
    [Export] public AudioStreamPlayer levelCompleted;
    [Export] public AudioStreamPlayer fadeOut;
    [Export] public AudioStreamPlayer fadeIn;


    public void PlayRandomSound(AudioStreamPlayer[] soundPool)
    {
        int index = random.Next(soundPool.Length);
        soundPool[index].Play();
    }

    // Music
    [ExportSubgroup("Music")]
    [Export] public AudioStreamPlayer musicPurpleForest;
    [Export] public AudioStreamPlayer musicDistantShore;
    [Export] public AudioStreamPlayer musicCaveOutskirts;
    [Export] public AudioStreamPlayer musicIslandsOfAshes;
    [Export] public AudioStreamPlayer musicVoid;
}
