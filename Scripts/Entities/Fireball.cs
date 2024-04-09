using Godot;
using System;

public partial class Fireball : CharacterBody2D
{
    private GameState gameState; // singleton

    [Export] Sprite2D spriteNode;
	const float speed = 200.0f;
    const float activationTime = 0.05f;
    //const float playerTeleportOffset = 2.0f;

    private float flyTime = 0.0f; 

	private float direction;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
    }

    public override void _PhysicsProcess(double delta)
	{
        flyTime += (float)delta;
        Velocity = new Vector2(direction * speed, 0.0f);

        var collision = MoveAndCollide(Velocity * (float)delta);
        if (collision != null)
        {
            if ((collision.GetCollider() as Node).IsInGroup("PlayerCollider"))
            {
                QueueFree();

                if (flyTime > activationTime)
                {
                    //Vector2 playerOffset = new(-direction * playerTeleportOffset, 0.0f);
                    //gameState.SetPlayerPosition(GlobalPosition + playerOffset);
                    gameState.SetPlayerPosition(GlobalPosition);
                }
            }
        }
    }

	public void SetDirection(bool right)
	{
        direction = right ? 1.0f : -1.0f;
		spriteNode.FlipH = !right;
    }

	void _OnVisibleOnScreenNotifier2dScreenExited()
	{
        GD.Print("Exited Screen");
        QueueFree();
    }
}
