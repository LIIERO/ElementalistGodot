using Godot;
using System;

public partial class MenuBackgroundMovement : ParallaxBackground
{
	[Export] private float speed = 10f;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		ScrollOffset += Vector2.One * (float)delta * speed;
	}
}
