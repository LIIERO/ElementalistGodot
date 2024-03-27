using Godot;
using System;

public partial class Fireball : Area2D
{
    private GameState gameState; // singleton
    //private StaticBody2D hitbox;

    [Export] AnimatedSprite2D spriteNode;
	const float speed = 200.0f;
    const float activationTime = 0.05f;
    const float playerTeleportOffset = 8.0f;

    private float flyTime = 0.0f; 

	private float direction;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        //hitbox = GetNode<StaticBody2D>("Hitbox");
    }

    public override void _Process(double delta)
	{
        flyTime += (float)delta;
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

            if (flyTime > activationTime)
            {
                Vector2 playerOffset = new(-direction * playerTeleportOffset, 0.0f);
                gameState.SetPlayerPosition(GlobalPosition + playerOffset);
            }
                
		}
	}

	void _OnVisibleOnScreenNotifier2dScreenExited()
	{
        QueueFree();
    }
}
