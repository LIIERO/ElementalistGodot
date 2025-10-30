using GlobalTypes;
using Godot;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using static System.Net.Mime.MediaTypeNames;

public partial class LevelTeleport : Interactable
{
    // Singletons
    private GameState gameState;
    private CustomSignals customSignals;
    private LevelTransitions levelTransitions;

    private AnimatedSprite2D currentSprite;
    private Label teleportText;
    private Label levelNameDisplay;
    private Label levelNameDisplayShadow;
    private AnimationPlayer nameDisplayAnimation;

    //private PackedScene yesNoScreen = null;
    private YesNoScreen abilityRetrievalApprovalPopup = null;

    [Export] private LevelData levelToTeleportTo;
    [Export] private bool swapLevelNameColours = false;
    [Export] private bool elementalShellRequired = false;

    public static bool setPlayerLevelEnterPosition = false;
    private List<ElementState> tempRetrievedElements = null;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PopupResult, new Callable(this, MethodName.RetrievalPopupResult));
        
        currentSprite = GetNode<AnimatedSprite2D>("MovedByAnimation/AnimatedSprite2D");
        teleportText = GetNode<Label>("MovedByAnimation/Text/Label");
        nameDisplayAnimation = GetNode<AnimationPlayer>("InfoAnimation");
        levelNameDisplay = GetNode<Label>("InfoCard");
        levelNameDisplayShadow = levelNameDisplay.GetNode<Label>("Shadow");
        levelNameDisplay.Text = gameState.GetLevelName(levelToTeleportTo.Name);
        levelNameDisplayShadow.Text = gameState.GetLevelName(levelToTeleportTo.Name);
        levelNameDisplay.Hide();
        if (swapLevelNameColours)
            SwapLevelNameColors();
        if (levelToTeleportTo.IsSalvageable)
        {
            GetNode<Sprite2D>("InfoCard/ElementalShellImage").Show();
            //yesNoScreen = ResourceLoader.Load<PackedScene>("res://Scenes/UI/YesNoScreen.tscn"); // Popup only needed for Salvageable levels
        }
            
        

        base._Ready();

        teleportText.Text = levelToTeleportTo.ID[0].ToString();

        // Not enough completed levels or not completed necessary level - teleport not showing up
        if (gameState.GetNoLocalCompletedStandardLevels() < levelToTeleportTo.NoCompletedToUnlock || (levelToTeleportTo.SpecificLevelCompletedToUnlock != string.Empty && !gameState.HasLevelBeenCompleted(levelToTeleportTo.SpecificLevelCompletedToUnlock)))
        {
            QueueFree();
            return;
        }

        if (elementalShellRequired && !gameState.IsAbilitySalvagingUnlocked) // Special case - softlock prevention for one level in void
        {
            QueueFree();
            return;
        }

        if (gameState.HasLevelBeenCompleted(levelToTeleportTo.ID)) // Level is completed - gold outline
        {
            if (levelToTeleportTo.IsSpecial)
                currentSprite.Play("LevelCompletedSpecial");
            else if (levelToTeleportTo.IsSalvageable)
                currentSprite.Play("LevelCompletedSalvage");
            else currentSprite.Play("LevelCompleted");
        }
        else // Level is not completed - no outline
        {
            if (levelToTeleportTo.IsSpecial)
                currentSprite.Play("LevelNotCompletedSpecial");
            else if (levelToTeleportTo.IsSalvageable)
                currentSprite.Play("LevelNotCompletedSalvage");
            else currentSprite.Play("LevelNotCompleted");
        }
        

        // Change player position according to level they are coming from, when entering a hub
        // setPlayerLevelEnterPosition set to true in GameState
        if (setPlayerLevelEnterPosition && levelToTeleportTo.ID == gameState.PreviousLevel)
        {
            gameState.PlayerHubRespawnPosition = GlobalPosition;
            setPlayerLevelEnterPosition = false;
            gameState.SetPlayerPosition(GlobalPosition);
        }
    }

    private void SwapLevelNameColors()
    {
        //Color frontColor = (Color)levelNameDisplay.Get("theme_override_colors/font_color");
        //Color shadowColor = (Color)levelNameDisplayShadow.Get("theme_override_colors/font_color");
        //levelNameDisplay.Set("theme_override_colors/font_color", shadowColor);
        //levelNameDisplayShadow.Set("theme_override_colors/font_color", frontColor);

        levelNameDisplayShadow.ZIndex = 1;
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

        // Salvaged ability retrieval system
        if (levelToTeleportTo.IsSalvageable && gameState.RetrieveAbilitiesSalvagedInLevel(levelToTeleportTo.Name, out List<ElementState> retrievedElements))
        {
            tempRetrievedElements = retrievedElements;

            abilityRetrievalApprovalPopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/YesNoScreen.tscn").Instantiate() as YesNoScreen;
            AddChild(abilityRetrievalApprovalPopup);
            abilityRetrievalApprovalPopup.SetText(gameState.UITextData["ability_retrieval_question"]);
            abilityRetrievalApprovalPopup.CreatePopup(popupId:levelToTeleportTo.Name);
        }
        else
        {
            EnterLevel();
        }
    }

    private void EnterLevel()
    {
        gameState.SalvagedAbilities = new();

        if (playerScriptReference.IsHoldingGoal)
        {
            // Ability salvaging with Fractured Fragment
            if (gameState.IsAbilitySalvagingUnlocked && playerScriptReference.AbilityList.Count > 0)
                gameState.SalvagedAbilities = new List<ElementState>(playerScriptReference.AbilityList);
        }

        levelTransitions.StartLevelTransition(levelToTeleportTo);
        if (levelToTeleportTo.IsSpecial) gameState.IsCurrentLevelSpecial = true;
    }

    private void RetrievalPopupResult(bool areYouSure, string popupId)
    {
        if (popupId != levelToTeleportTo.Name) return; // So only one portal receives the event (using name as id)

        abilityRetrievalApprovalPopup = null;

        if (areYouSure)
        {
            gameState.SalvagedAbilities = tempRetrievedElements;
            tempRetrievedElements = null;
            gameState.PlayerHubRespawnPosition = GlobalPosition;
            playerScriptReference.RestartLevel(false);
        }
        else
        {
            EnterLevel();
        }
    }
}
