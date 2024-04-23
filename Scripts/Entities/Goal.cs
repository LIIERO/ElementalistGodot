using GlobalTypes;
using Godot;
using System;

public partial class Goal : Area2D
{
    [Export] public bool IsSpecial { get; private set; } = false;

    const float smoothTime = 0.3f;

    // Singletons
    private CustomSignals customSignals;
    private GameState gameState;
    private AudioManager audioManager;

    private Node2D objectToFollow = null;
    private Vector2 initialPosition;

    [Export] private AnimatedSprite2D sprite;
    [Export] private Light2D backgroundLight;

    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    private bool assigned = false;

    public void AssignObjectToFollow(Node2D player)
    {
        if (assigned || gameState.IsLevelTransitionPlaying) return;
        assigned = true;
        objectToFollow = player;
        audioManager.sunCollectSound.Play();
    }

    public void DetatchFromObjectToFollow()
    {
        objectToFollow = null;
    }

    void _OnBodyEntered(Node2D player)
    {
        if (player is not Player) return;
        (player as Player).IsHoldingGoal = true;

        if (IsSpecial) (player as Player).IsHoldingSpecialGoal = true;

        AssignObjectToFollow(player);
    }

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        customSignals.Connect(CustomSignals.SignalName.PlayerDied, new Callable(this, MethodName.DetatchFromObjectToFollow));
        initialPosition = Position;

        bool lightActive = GetNode<SettingsManager>("/root/SettingsManager").LightParticlesActive;
        if (!lightActive)
        {
            backgroundLight.QueueFree();
        }

        SetAppearance(lightActive);
    }

    private void SetAppearance(bool lightActive)
    {
        if (gameState.HasCurrentLevelBeenCompleted()) // White goal
        {
            if (gameState.IsCurrentLevelSpecial) sprite.Play("LevelCompletedSpecial");
            else sprite.Play("LevelCompleted");

            if (lightActive) backgroundLight.Color = GameUtils.ColorsetToColor[ColorSet.white];
        }

        else // Yellow goal (or red if special)
        {
            if (gameState.IsCurrentLevelSpecial)
            {
                sprite.Play("LevelNotCompletedSpecial");
                if (lightActive) backgroundLight.Color = GameUtils.ColorsetToColor[ColorSet.red];
            }
            else
            {
                sprite.Play("LevelNotCompleted");
                if (lightActive) backgroundLight.Color = GameUtils.ColorsetToColor[ColorSet.yellow];
            }   
        }
    }

    public override void _Process(double delta)
    {
        Vector2 desiredPosition;
        if (objectToFollow == null)
        {
            desiredPosition = initialPosition;
        }
        else
        {
            desiredPosition = objectToFollow.Position + new Vector2(0f, -2.5f * GameUtils.gameUnitSize); 
        }

        float smoothedX = GameUtils.SmoothDamp(Position.X, desiredPosition.X, ref velocityX, smoothTime, 200, (float)delta);
        float smoothedY = GameUtils.SmoothDamp(Position.Y, desiredPosition.Y, ref velocityY, smoothTime, 200, (float)delta);
        Position = new Vector2(smoothedX, smoothedY);
    }
}
