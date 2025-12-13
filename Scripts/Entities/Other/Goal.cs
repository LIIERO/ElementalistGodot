using GlobalTypes;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Goal : Area2D, IUndoable
{
    [Export] public bool IsSpecial { get; private set; } = false;

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

    // Undo system
    private List<bool> goalStateCheckpoints = new List<bool>();
    private bool isHolding = false;
    private bool assigned = false; // Gets set to true after getting collected and stays true (unless undone)

    public void AssignObjectToFollow(Node2D player)
    {
        if (assigned || gameState.IsLevelTransitionPlaying) return;
        assigned = true;
        isHolding = true;
        objectToFollow = player;
        audioManager.sunCollectSound.Play();
        customSignals.EmitSignal(CustomSignals.SignalName.RequestCheckpoint);
    }

    public void DetatchFromObjectToFollow()
    {
        if (objectToFollow is Player player)
        {
            player.IsHoldingGoal = false;
            player.IsHoldingSpecialGoal = false;
        }
        objectToFollow = null;
        isHolding = false;
    }

    void _OnBodyEntered(Node2D obj)
    {
        if (obj is not Player player) return;

        player.IsHoldingGoal = true;

        if (IsSpecial) player.IsHoldingSpecialGoal = true;

        AssignObjectToFollow(obj);
    }

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        //customSignals.Connect(CustomSignals.SignalName.PlayerDied, new Callable(this, MethodName.DetatchFromObjectToFollow));
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.ReplaceTopCheckpoint, new Callable(this, MethodName.ReplaceTopLocalCheckpoint));
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
        if (gameState.IsHubLoaded()) // HUB Fragment for meta puzzles
        {
            sprite.Play("LevelCompleted");
            if (lightActive) backgroundLight.Color = GameUtils.ColorsetToColor[ColorSet.white];
            return;
        }

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
        bool absorbeAnimation = gameState.IsLevelTransitionPlaying && assigned;

        Vector2 desiredPosition;
        if (objectToFollow == null)
        {
            desiredPosition = initialPosition;
        }
        else
        {
            float yOffset = absorbeAnimation ? -0.5f : -2.5f;
            desiredPosition = objectToFollow.Position + new Vector2(0f, yOffset * GameUtils.gameUnitSize); 
        }

        int maxSpeed = absorbeAnimation ? 2000 : 200;
        float smoothTime = 0.3f;
        float smoothedX = GameUtils.SmoothDamp(Position.X, desiredPosition.X, ref velocityX, smoothTime, maxSpeed, (float)delta);
        float smoothedY = GameUtils.SmoothDamp(Position.Y, desiredPosition.Y, ref velocityY, smoothTime, maxSpeed, (float)delta);
        Position = new Vector2(smoothedX, smoothedY);

        // Proximity fadeout on level complete (I dont think imma use it tbh)
        /*if (!absorbeAnimation) return;
        float playerFragmentDistance = Position.DistanceTo(desiredPosition);
        float thresholdDistance = 4.0f * GameUtils.gameUnitSize;
        if (playerFragmentDistance < thresholdDistance)
        {
            float alphaFactor = playerFragmentDistance / thresholdDistance;
            Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f * alphaFactor);
        }*/
    }

    public void AddLocalCheckpoint()
    {
        goalStateCheckpoints.Add(isHolding);
    }

    public void UndoLocalCheckpoint(bool nextCpRequested)
    {
        if (!nextCpRequested && goalStateCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(goalStateCheckpoints);
        isHolding = goalStateCheckpoints[^1];

        if (!isHolding)
        {
            DetatchFromObjectToFollow();
            GlobalPosition = initialPosition;
            RemoveAssignedAfterDelay();
        }
    }

    public void ReplaceTopLocalCheckpoint()
    {
        GameUtils.ListRemoveLastElement(goalStateCheckpoints);
        AddLocalCheckpoint();
    }

    async private void RemoveAssignedAfterDelay() // If it's done too fast player can get the goal when not supposed to
    {
        await ToSignal(GetTree().CreateTimer(0.5f, processInPhysics: true), "timeout");
        assigned = false;
    }
}
