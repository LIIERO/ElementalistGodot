using Godot;
using System;

public partial class MenuCamera : Camera2D
{
	float speed = 50.0f;

	public override void _Process(double delta)
	{
		Position = new Vector2(Position.X + (speed * (float)delta), Position.Y);
	}
}
