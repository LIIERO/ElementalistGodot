using Godot;
using System;

public partial class RandomAnimationOffset : AnimationPlayer
{
    [Export] private string animationName = "Idle";
    [Export] private float maxOffset = 2.9f;
	
	public override void _Ready()
	{
        RandomNumberGenerator rng = new();
        float randomOffset = rng.RandfRange(0.0f, maxOffset);
        Play(animationName);
        Seek(randomOffset);
    }
}
