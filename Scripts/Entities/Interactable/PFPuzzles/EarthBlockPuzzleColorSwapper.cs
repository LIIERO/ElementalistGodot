using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class EarthBlockPuzzleColorSwapper : Interactable
{
    private CustomSignals customSignals;
    private AudioManager audioManager;


    public override void _Ready()
	{
		base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
    }

    protected override void Interact()
	{
		base.Interact();
        audioManager.buttonSelected.Play();
        customSignals.EmitSignal(CustomSignals.SignalName.SwapEarthBlockColor);
    }
}
