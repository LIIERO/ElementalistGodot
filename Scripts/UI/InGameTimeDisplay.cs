using Godot;
using System;

public partial class InGameTimeDisplay : Node2D
{
	private Label currentLevelLabel;

	private GameState gameState;
    private SettingsManager settingsManager;
    //private CustomSignals customSignals;

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        currentLevelLabel = GetNode<Label>("Label");

        if (settingsManager.SpeedrunTimerVisible) Show();
        else Hide();
	}

    public override void _Process(double delta)
    {
        if (!gameState.IsLevelTransitionPlaying)
        {
            gameState.InGameTime += delta;
        }

        currentLevelLabel.Text = GameUtils.FormatTime(gameState.InGameTime);
    }
}
