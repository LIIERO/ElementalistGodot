using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Gate : Node2D
{
    protected static bool IsAnyGateMovingPrev = false; 
    private static bool IsAnyGateMovingNext = false;
    public static bool IsAnyGateMoving => IsAnyGateMovingPrev || IsAnyGateMovingNext; // To fix a bug where one gate stops moving and the value is false for one frame

    // Singletons
    protected GameState gameState; 
    protected CustomSignals customSignals;
    protected AudioManager audioManager;

    protected AnimationPlayer animator;
    protected Label requiredFragmentsDisplay;
    protected Sprite2D gateSprite;

    protected bool isOpened = false;
    //protected bool isMoving = false;

    // Undo system
    private List<bool> gateStateCheckpoints = new List<bool>();

    public override void _Ready()
	{
        IsAnyGateMovingPrev = false;

        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");
        gateSprite = GetNode<Sprite2D>("ToMove/Sprite2D");
    }

    public override void _Process(double delta)
    {
        if (animator.CurrentAnimation == "Open") IsAnyGateMovingPrev = true;

        IsAnyGateMovingNext = IsAnyGateMovingPrev;
    }

    protected virtual void Open()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = true;
        IsAnyGateMovingPrev = true;
        animator.Play("Open", customSpeed:0.8f);
        gateSprite.Modulate = new Color(1.0f, 1.0f, 0.5f);
    }

    protected virtual void Reset()
    {
        isOpened = false;
        IsAnyGateMovingPrev = false;
        animator.Play("Reset");
        gateSprite.Modulate = new Color(1.0f, 1.0f, 1.0f);
    }

    protected virtual void AddLocalCheckpoint()
    {
        gateStateCheckpoints.Add(isOpened);
    }

    protected virtual void UndoLocalCheckpoint(bool nextCpRequested)
    {
        IsAnyGateMovingPrev = false;

        if (!nextCpRequested && gateStateCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(gateStateCheckpoints);
        isOpened = gateStateCheckpoints[^1];

        if (!isOpened) Reset();
    }

    void _on_animation_player_animation_finished(string animationName)
    {
        if (animationName == "Open") IsAnyGateMovingPrev = false;
    }
}
