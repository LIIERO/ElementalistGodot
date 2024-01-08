using Godot;
using System;

public partial class Fireball : Area2D
{
	[Export] AnimatedSprite2D spriteNode;
	const float speed = 200.0f;

	private float direction;

	public override void _Process(double delta)
	{
        Position += new Vector2((float)delta * direction * speed, 0.0f);
    }

	public void SetDirection(bool right)
	{
        direction = right ? 1.0f : -1.0f;
		spriteNode.FlipH = !right;
    }

    void _OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("PlayerCollider"))
		{
			QueueFree();
		}
	}

	void _OnVisibleOnScreenNotifier2dScreenExited()
	{
        QueueFree();
    }
}
