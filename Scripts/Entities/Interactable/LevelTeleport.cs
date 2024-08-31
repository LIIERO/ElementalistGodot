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
    private Label levelNameDisplay;
    private AnimationPlayer nameDisplayAnimation;

    [Export] private LevelData levelToTeleportTo;

    public static bool setPlayerLevelEnterPosition = false;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        currentSprite = GetNode<AnimatedSprite2D>("MovedByAnimation/AnimatedSprite2D");
        teleportText = GetNode<Label>("MovedByAnimation/Text/Label");
        nameDisplayAnimation = GetNode<AnimationPlayer>("InfoAnimation");
        levelNameDisplay = GetNode<Label>("InfoCard");
        levelNameDisplay.Text = levelToTeleportTo.Name;
        levelNameDisplay.GetNode<Label>("Shadow").Text = levelToTeleportTo.Name;
        levelNameDisplay.Hide();
        base._Ready();

        teleportText.Text = levelToTeleportTo.ID[0].ToString();

        // Not enough completed levels or not completed necessary level - teleport not showing up
        if (gameState.GetNoLocalCompletedStandardLevels() < levelToTeleportTo.NoCompletedToUnlock || (levelToTeleportTo.SpecificLevelCompletedToUnlock != string.Empty && !gameState.HasLevelBeenCompleted(levelToTeleportTo.SpecificLevelCompletedToUnlock)))
        {
            QueueFree();
        }
        else
        {
            if (gameState.HasLevelBeenCompleted(levelToTeleportTo.ID)) // Level is completed - gold outline
            {
                if (levelToTeleportTo.IsSpecial)
                    currentSprite.Play("LevelCompletedSpecial");
                else currentSprite.Play("LevelCompleted");
            }
            else // Level is not completed - no outline
            {
                if (levelToTeleportTo.IsSpecial)
                    currentSprite.Play("LevelNotCompletedSpecial");
                else currentSprite.Play("LevelNotCompleted");
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

    protected override void PlayerEntered()
    {
        base.PlayerEntered();
        levelNameDisplay.Show();
        nameDisplayAnimation.Play("Appear");
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();
        //levelNameDisplay.Hide();
        //nameDisplayAnimation.CurrentAnimationPosition
        nameDisplayAnimation.PlayBackwards("Appear");
    }

    protected override void Interact()
    {
        base.Interact();
        levelTransitions.StartLevelTransition(levelToTeleportTo);
        if (levelToTeleportTo.IsSpecial) gameState.IsCurrentLevelSpecial = true;
    }
}
