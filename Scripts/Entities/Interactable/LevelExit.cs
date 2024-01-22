using Godot;
using System;


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

        if (playerScriptReference.IsHoldingGoal)
            gameState.CompleteCurrentLevel();

        levelTransitions.StartHubTransition(playerScriptReference.IsHoldingGoal);  
    }
}
