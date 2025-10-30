using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class EarthBlockPuzzleColorSwapper : Interactable
{
    private CustomSignals customSignals; // Singleton


    public override void _Ready()
	{
		base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    protected override void Interact()
	{
		base.Interact();

        customSignals.EmitSignal(CustomSignals.SignalName.SwapEarthBlockColor);
    }
}
