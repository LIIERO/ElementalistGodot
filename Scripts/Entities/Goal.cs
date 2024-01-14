using GlobalTypes;
using Godot;
using System;

public partial class Goal : Area2D
{
    const float smoothTime = 0.3f;

    private GameState gameState;
    private Node2D objectToFollow = null;

    [Export] private AnimatedSprite2D sprite;
    [Export] private Light2D backgroundLight;

    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    public void AssignObjectToFollow(Node2D obj)
    {
        objectToFollow = obj;
    }

    void _OnBodyEntered(Node2D player)
    {
        if (player is not Player) return;

        AssignObjectToFollow(player);
        (player as Player).IsHoldingGoal = true;
    }

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");

        if (gameState.HasCurrentLevelBeenCompleted()) // White goal
        {
            sprite.Play("LevelCompleted");
            backgroundLight.Color = GameUtils.ColorsetToColor[ColorSet.white];
        }

        else // Yellow goal
        {
            sprite.Play("LevelNotCompleted");
            backgroundLight.Color = GameUtils.ColorsetToColor[ColorSet.yellow];
        }
    }

    public override void _Process(double delta)
    {
        if (objectToFollow != null)
        {
            Vector2 desiredPosition = objectToFollow.Position + new Vector2(0f, -2.5f * GameUtils.gameUnitSize);
            float smoothedX = GameUtils.SmoothDamp(Position.X, desiredPosition.X, ref velocityX, smoothTime, 200, (float)delta);
            float smoothedY = GameUtils.SmoothDamp(Position.Y, desiredPosition.Y, ref velocityY, smoothTime, 200, (float)delta);
            Position = new Vector2(smoothedX, smoothedY);
        }
    }
}
