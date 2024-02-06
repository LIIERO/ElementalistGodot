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

    [ExportSubgroup("Entities")]
    [Export] public AudioStreamPlayer orbCollectSound;

    [ExportSubgroup("UI")]
    [Export] public AudioStreamPlayer buttonSelected;


    public void PlayRandomSound(AudioStreamPlayer[] soundPool)
    {
        int index = random.Next(soundPool.Length);
        soundPool[index].Play();
    }
}
