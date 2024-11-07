using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Gate : Node2D, IUndoable
{
    //protected static bool IsAnyGateMovingPrev = false; 
    //private static bool IsAnyGateMovingNext = false;
    //public static bool IsAnyGateMoving => IsAnyGateMovingPrev || IsAnyGateMovingNext; // To fix a bug where one gate stops moving and the value is false for one frame

    public bool IsMovingUp { get; protected set; } = false;

    public const float openAnimationSpeed = 0.6f;

    // Singletons
    protected GameState gameState; 
    protected CustomSignals customSignals;
    protected AudioManager audioManager;

    protected AnimationPlayer animator;
    protected Label requiredFragmentsDisplay;
    protected Sprite2D gateSprite;

    protected bool isOpened = false;

    // Undo system
    private List<bool> gateStateCheckpoints = new List<bool>();

    public override void _Ready()
	{
        IsMovingUp = false;

        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.ReplaceTopCheckpoint, new Callable(this, MethodName.ReplaceTopLocalCheckpoint));
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");
        gateSprite = GetNode<Sprite2D>("ToMove/Sprite2D");
    }

    /*public override void _Process(double delta)
    {
        if (animator.CurrentAnimation == "Open") IsAnyGateMovingPrev = true;

        IsAnyGateMovingNext = IsAnyGateMovingPrev;
    }*/

    protected virtual void Open()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = true;
        IsMovingUp = false;
        animator.Play("Open", customSpeed: openAnimationSpeed);
        gateSprite.Modulate = new Color(1.0f, 1.0f, 0.5f);
    }

    protected virtual void Reset()
    {
        isOpened = false;
        IsMovingUp = false;
        animator.Play("Reset");
        gateSprite.Modulate = new Color(1.0f, 1.0f, 1.0f);
    }

    public virtual void AddLocalCheckpoint()
    {
        gateStateCheckpoints.Add(isOpened);
    }

    public virtual void UndoLocalCheckpoint(bool nextCpRequested)
    {
        IsMovingUp = false;

        if (!nextCpRequested && gateStateCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(gateStateCheckpoints);
        isOpened = gateStateCheckpoints[^1];

        if (!isOpened) Reset();
    }

    public virtual void ReplaceTopLocalCheckpoint()
    {
        GameUtils.ListRemoveLastElement(gateStateCheckpoints);
        gateStateCheckpoints.Add(isOpened);
    }

    void _on_animation_player_animation_finished(string animationName)
    {
        if (animationName == "Open") IsMovingUp = false;
    }
}
