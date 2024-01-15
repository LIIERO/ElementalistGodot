using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class LevelTeleport : Interactable
{
    private GameState gameState;

    [Export] private AnimatedSprite2D currentSprite;
    [Export] private Label teleportText;
    // TODO: Make a resource to store level data
    [Export] private int noCompletedToUnlock = 0;
    [Export] private string id = "";
    [Export] private string name = "";
    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        base._Ready();

        teleportText.Text = id[0].ToString();
        if (gameState.NoCompletedLevels < noCompletedToUnlock) // Not enough completed levels - teleport not showing up
        {
            QueueFree();
        }
        else
        {
            if (gameState.HasLevelBeenCompleted(id)) // Level is completed - gold outline
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
        gameState.LoadLevel(id);
    }
}
