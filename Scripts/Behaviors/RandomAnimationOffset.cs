using Godot;
using System;

public partial class RandomAnimationOffset : AnimationPlayer
{
    [Export] private string animationName = "Idle";
    [Export] private float maxOffset = 2.9f;
    [Export] private bool useSetOffsetInstead = false;
    [Export] private float setOffset = 0.0f;
	
	public override void _Ready()
	{
        if (useSetOffsetInstead)
        {
            Play(animationName);
            Seek(setOffset);
            return;
        }

        RandomNumberGenerator rng = new();
        float randomOffset = rng.RandfRange(0.0f, maxOffset);
        Play(animationName);
        Seek(randomOffset);
    }
}
