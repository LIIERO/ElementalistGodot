using Godot;
using System;

public partial class ParallaxOffset : ParallaxBackground
{
    [Export] private float maxOffset = 448.0f;
    //[Export] private float offset = 0.0f;

    public override void _Ready()
	{
        RandomNumberGenerator rng = new();
        float offset = rng.RandfRange(0.0f, maxOffset);
        ScrollBaseOffset = new Vector2(2*offset, 0.0f);
	}
}
