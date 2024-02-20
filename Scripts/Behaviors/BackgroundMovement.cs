using Godot;
using System;

public partial class BackgroundMovement : ParallaxLayer
{
	[Export] private float speedX = 10f;
    [Export] private float speedY = 10f;

	public override void _Process(double delta)
	{
		MotionOffset += new Vector2(speedX, speedY) * (float)delta;
	}
}
