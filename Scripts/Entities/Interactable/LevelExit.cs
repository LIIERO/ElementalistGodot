using GlobalTypes;
using Godot;
using System;
using System.Collections.Generic;


public partial class LevelExit : Interactable
{
    private GameState gameState;
    private LevelTransitions levelTransitions;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        base._Ready();
    }

    protected override void Interact()
    {
        base.Interact();

        gameState.SalvagedAbilities = new();

        if (playerScriptReference.IsHoldingGoal)
        {
            // Ability salvaging
            if (gameState.IsAbilitySalvagingUnlocked && playerScriptReference.AbilityList.Count > 0)
            {
                gameState.SalvagedAbilities = new List<ElementState>(playerScriptReference.AbilityList);
                gameState.UpdateAbilitiesSalvagedInLevels(playerScriptReference.AbilityList);
            }   
            
            gameState.CompleteCurrentLevel();
        }
            
        levelTransitions.StartHubTransition(playerScriptReference.IsHoldingGoal);  
    }
}
