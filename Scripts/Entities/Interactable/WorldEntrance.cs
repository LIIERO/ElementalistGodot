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

    [Export] bool showCardDownwards = false;
    [Export] WorldData worldToTeleportTo;

    public static bool setPlayerWorldEnterPosition = false;

    // Info card
    private Sprite2D infoCard;
    private AnimationPlayer infoDisplayAnimation;
    private string appearAnimationName;

    public override void _Ready()
    {
        base._Ready();

        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        outlineSprite = GetNode<AnimatedSprite2D>("Foreground");
        infoDisplayAnimation = GetNode<AnimationPlayer>("InfoAnimation");

        infoCard = GetNode<Sprite2D>("InfoCard");
        infoCard.GetNode<Label>("WorldNumber").Text = $"World {worldToTeleportTo.ID}";
        infoCard.GetNode<Label>("WorldName").Text = gameState.GetLevelName(worldToTeleportTo.Name);
        infoCard.GetNode<Label>("LevelsCompleted").Text = $"{gameState.GetNoCompletedStandardLevelsInWorld(worldToTeleportTo.ID)}/{gameState.GetNoStandardLevelsInWorld(worldToTeleportTo.ID)}";
        infoCard.GetNode<Label>("SpecialLevelsCompleted").Text = $"{gameState.GetNoCompletedSpecialLevelsInWorld(worldToTeleportTo.ID)}/{gameState.GetNoSpecialLevelsInWorld(worldToTeleportTo.ID)}";
        infoCard.Hide();

        appearAnimationName = showCardDownwards ? "AppearDown" : "Appear";

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
        infoDisplayAnimation.Play(appearAnimationName);
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();
        infoDisplayAnimation.PlayBackwards(appearAnimationName);
    }

    protected override void Interact()
    {
        base.Interact();

        gameState.SalvagedAbilities = new();

        levelTransitions.StartWorldTransition(worldToTeleportTo);
    }

}
