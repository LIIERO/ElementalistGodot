using Godot;
using System;

public partial class AudioManager : Node
{

    readonly private Random random = new();
    public override void _Ready()
    {
        
    }

    [ExportSubgroup("Player")]
    [Export] public AudioStreamPlayer[] softFootsteps;
    [Export] public AudioStreamPlayer waterAbility;
    [Export] public AudioStreamPlayer airAbility;


    [ExportSubgroup("Entities")]
    [Export] public AudioStreamPlayer orbCollectSound;
    [Export] public AudioStreamPlayer sunCollectSound;

    [ExportSubgroup("UI")]
    [Export] public AudioStreamPlayer buttonSelected;

    [ExportSubgroup("Transitions")]
    [Export] public AudioStreamPlayer levelCompleted;


    public void PlayRandomSound(AudioStreamPlayer[] soundPool)
    {
        int index = random.Next(soundPool.Length);
        soundPool[index].Play();
    }
}
