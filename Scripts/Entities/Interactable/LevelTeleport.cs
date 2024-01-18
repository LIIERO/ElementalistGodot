using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class LevelTeleport : Interactable
{
    private GameState gameState; // Singleton
    private AnimatedSprite2D currentSprite;
    private Label teleportText;

    [Export] private LevelData levelData;
    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        currentSprite = GetNode<AnimatedSprite2D>("MovedByAnimation/AnimatedSprite2D");
        teleportText = GetNode<Label>("MovedByAnimation/Text/Label");
        base._Ready();

        teleportText.Text = levelData.ID[0].ToString();
        if (gameState.NoCompletedLevels < levelData.NoCompletedToUnlock) // Not enough completed levels - teleport not showing up
        {
            QueueFree();
        }
        else
        {
            if (gameState.HasLevelBeenCompleted(levelData.ID)) // Level is completed - gold outline
            {
                currentSprite.Play("LevelCompleted");
            }
            else // Level is not completed - no outline
            {
                currentSprite.Play("LevelNotCompleted");
            }
        }
    }

    protected override void Interact()
    {
        base.Interact();
        gameState.PlayerHubPosition = GlobalPosition; // Upon returning player should respawn on top of the teleport they entered
        gameState.LoadLevel(levelData.ID);
    }
}
