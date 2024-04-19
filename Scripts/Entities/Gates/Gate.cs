using Godot;
using System;

public abstract partial class Gate : Node2D
{
    // Singletons
    protected GameState gameState; 
    protected CustomSignals customSignals;
    protected AudioManager audioManager;

    protected AnimationPlayer animator;
    protected Label requiredFragmentsDisplay;

    protected bool isOpened = false;

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");
    }

    protected void Open()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = true;
        animator.Play("Open");
    }
}
