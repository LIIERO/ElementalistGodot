using Godot;
using System;

public partial class CurrentLevelDisplay : Node2D
{
	private Label currentLevelLabel;

	private GameState gameState;
    private CustomSignals customSignals;
    
    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.GamePaused, new Callable(this, MethodName.OnPauseAndResume));
        currentLevelLabel = GetNode<Label>("Label");
        Hide();

        currentLevelLabel.Text = "Level " + gameState.CurrentLevel + ": " + gameState.GetLevelName(gameState.CurrentLevelNameID);
	}

    void OnPauseAndResume(bool pause)
    {
        if (pause && !gameState.IsHubLoaded())
            Show();
        else
            Hide();
    }
}
