using GlobalTypes;
using Godot;
using System;

public partial class WorldEntrance : Interactable
{
    // Singletons
    //private CustomSignals customSignals;
    private GameState gameState;
    private LevelTransitions levelTransitions;

    private AnimatedSprite2D outlineSprite;
    private Node2D playerRespawnPosition;

    [Export] WorldData worldToTeleportTo;

    public static bool setPlayerWorldEnterPosition = false;

    public override void _Ready()
    {
        base._Ready();

        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        outlineSprite = GetNode<AnimatedSprite2D>("Foreground");
        playerRespawnPosition = GetNode<Node2D>("PlayerRespawnPosition");

        if (gameState.HasWorldBeenCompleted(worldToTeleportTo.ID)) // Yellow outline
        {
            outlineSprite.Play("Completed");
        }
        else
        {
            outlineSprite.Play("NotCompleted");
        }

        // Change player position according to world they are coming from
        // setPlayerWorldEnterPosition set to true in GameState
        if (setPlayerWorldEnterPosition && worldToTeleportTo.ID == gameState.PreviousWorld)
        {
            setPlayerWorldEnterPosition = false;
            gameState.SetPlayerPosition(playerRespawnPosition.GlobalPosition);
        }
    }

    protected override void Interact()
    {
        base.Interact();

        levelTransitions.StartWorldTransition(worldToTeleportTo);
    }

}
