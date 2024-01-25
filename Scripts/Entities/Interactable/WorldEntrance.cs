using GlobalTypes;
using Godot;
using System;

public partial class WorldEntrance : Interactable
{
    // Singletons
    private CustomSignals customSignals;
    private GameState gameState;
    private LevelTransitions levelTransitions;

    private AnimatedSprite2D outlineSprite;

    [Export] WorldData worldToTeleportTo;

    public override void _Ready()
    {
        base._Ready();

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        outlineSprite = GetNode<AnimatedSprite2D>("Foreground");

        if (gameState.HasWorldBeenCompleted(worldToTeleportTo.ID)) // Yellow outline
        {
            outlineSprite.Play("Completed");
        }
        else
        {
            outlineSprite.Play("NotCompleted");
        }
    }

    protected override void Interact()
    {
        base.Interact();

        gameState.LoadWorld(worldToTeleportTo.ID);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

    }
}
