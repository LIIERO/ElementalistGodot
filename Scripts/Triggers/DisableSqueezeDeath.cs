using Godot;
using System;

public partial class DisableSqueezeDeath : Area2D
{
	void _OnBodyEntered(Node2D player)
	{
        if (player is not Player) return;
        (player as Player).SqueezeDeathDisabled = true;
    }

    void _OnBodyExited(Node2D player)
    {
        if (player is not Player) return;
        (player as Player).SqueezeDeathDisabled = false;
    }
}
