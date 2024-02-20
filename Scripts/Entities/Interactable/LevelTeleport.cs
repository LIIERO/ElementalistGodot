using Godot;
using System;
using System.Security.AccessControl;
using static System.Net.Mime.MediaTypeNames;

public partial class LevelTeleport : Interactable
{
    // Singletons
    private GameState gameState;
    //private CustomSignals customSignals;
    private LevelTransitions levelTransitions;

    private AnimatedSprite2D currentSprite;
    private Label teleportText;

    [Export] private LevelData levelToTeleportTo;

    public static bool setPlayerLevelEnterPosition = false;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        currentSprite = GetNode<AnimatedSprite2D>("MovedByAnimation/AnimatedSprite2D");
        teleportText = GetNode<Label>("MovedByAnimation/Text/Label");
        base._Ready();

        teleportText.Text = levelToTeleportTo.ID[0].ToString();
        if (gameState.GetNoLocalCompletedLevels() < levelToTeleportTo.NoCompletedToUnlock) // Not enough completed levels - teleport not showing up
        {
            QueueFree();
        }
        else
        {
            if (gameState.HasLevelBeenCompleted(levelToTeleportTo.ID)) // Level is completed - gold outline
            {
                currentSprite.Play("LevelCompleted");
            }
            else // Level is not completed - no outline
            {
                currentSprite.Play("LevelNotCompleted");
            }
        }

        // Change player position according to world they are coming from, when entering a hub
        // setPlayerLevelEnterPosition set to true in GameState
        if (setPlayerLevelEnterPosition && levelToTeleportTo.ID == gameState.PreviousLevel)
        {
            setPlayerLevelEnterPosition = false;
            gameState.SetPlayerPosition(GlobalPosition);
        }
    }

    protected override void Interact()
    {
        base.Interact();
        levelTransitions.StartLevelTransition(levelToTeleportTo); 
    }
}
