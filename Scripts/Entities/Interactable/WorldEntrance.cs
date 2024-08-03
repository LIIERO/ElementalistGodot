using GlobalTypes;
using Godot;
using System;

public partial class WorldEntrance : Interactable
{
    // Singletons
    //private CustomSignals customSignals;
    private GameState gameState;
    //private CustomSignals customSignals;
    private LevelTransitions levelTransitions;

    private AnimatedSprite2D outlineSprite;

    [Export] WorldData worldToTeleportTo;

    public static bool setPlayerWorldEnterPosition = false;

    // Info card
    private Sprite2D infoCard;

    public override void _Ready()
    {
        base._Ready();

        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        outlineSprite = GetNode<AnimatedSprite2D>("Foreground");

        infoCard = GetNode<Sprite2D>("InfoCard");
        infoCard.GetNode<Label>("WorldNumber").Text = $"World {worldToTeleportTo.ID}";
        infoCard.GetNode<Label>("WorldName").Text = worldToTeleportTo.Name;
        infoCard.GetNode<Label>("LevelsCompleted").Text = $"{gameState.GetNoCompletedStandardLevelsInWorld(worldToTeleportTo.ID)}/{gameState.GetNoStandardLevelsInWorld(worldToTeleportTo.ID)}";
        infoCard.GetNode<Label>("SpecialLevelsCompleted").Text = $"{gameState.GetNoCompletedSpecialLevelsInWorld(worldToTeleportTo.ID)}/{gameState.GetNoSpecialLevelsInWorld(worldToTeleportTo.ID)}";
        infoCard.Hide();

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
            gameState.PlayerHubRespawnPosition = GlobalPosition; // Changing worlds changes respawn position in hub
            gameState.SetPlayerPosition(GlobalPosition);
        }
    }

    protected override void PlayerEntered()
    {
        base.PlayerEntered();
        infoCard.Show();
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();
        infoCard.Hide();
    }

    protected override void Interact()
    {
        base.Interact();
        levelTransitions.StartWorldTransition(worldToTeleportTo);
    }

}
