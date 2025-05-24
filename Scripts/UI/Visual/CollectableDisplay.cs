using Godot;
using System;

public partial class CollectableDisplay : Node2D
{
	[Export] private Sprite2D elementalShell;
    [Export] private AnimatedSprite2D[] letters;

    private GameState gameState;
    private CustomSignals customSignals;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.GamePaused, new Callable(this, MethodName.OnPauseAndResume));

        OnPauseAndResume(false);
    }

    private void OnPauseAndResume(bool pause)
    {
        if (!gameState.IsAbilitySalvagingUnlocked && gameState.UnlockedLetters.Count == 0)
        {
            Hide();
            return;
        }

        if (pause)
        {
            Show();
        }
        else if (!gameState.IsHubLoaded())
        {
            Hide();
            return;
        }

        if (gameState.IsAbilitySalvagingUnlocked)
            elementalShell.Show();
        else
            elementalShell.Hide();

        for (int i = 0; i < GameUtils.wordToMake.Length; i++)
        {
            string l = GameUtils.wordToMake[i].ToString();
            if (gameState.UnlockedLetters.Contains(l))
            {
                letters[i].Play(l);
            }
            else
            {
                letters[i].Play("_");
            }
        }
    }
}
