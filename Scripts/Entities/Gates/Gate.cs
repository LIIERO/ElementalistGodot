using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Gate : Node2D
{
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
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");
        gateSprite = GetNode<Sprite2D>("ToMove/Sprite2D");
    }

    protected void Open()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = true;
        animator.Play("Open", customSpeed:0.8f);
        gateSprite.Modulate = new Color(1.0f, 1.0f, 0.5f);
    }

    protected void Reset()
    {
        isOpened = false;
        animator.Play("Reset");
        gateSprite.Modulate = new Color(1.0f, 1.0f, 1.0f);
    }

    protected virtual void AddLocalCheckpoint()
    {
        gateStateCheckpoints.Add(isOpened);
    }

    protected virtual void UndoLocalCheckpoint(bool nextCpRequested)
    {
        if (!nextCpRequested && gateStateCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(gateStateCheckpoints);
        isOpened = gateStateCheckpoints[^1];

        if (!isOpened) Reset();
    }
}
