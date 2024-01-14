using Godot;
using System;


public partial class LevelExit : Interactable
{
    private GameState gameState;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        base._Ready();
    }

    protected override void Interact()
    {
        base.Interact();

        if (playerScriptReference.IsHoldingGoal)
            gameState.CompleteCurrentLevel();

        gameState.LoadHub();
        
    }
}
