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
    protected Sprite2D gateSprite;

    protected bool isOpened = false;

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");
        gateSprite = GetNode<Sprite2D>("ToMove/Sprite2D");
    }

    protected void Open()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = true;
        animator.Play("Open");
        gateSprite.Modulate = new Color(1.0f, 1.0f, 0.75f);
    }
}
