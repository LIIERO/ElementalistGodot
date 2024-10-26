using Godot;
using System;

public partial class InGameTimeDisplay : Node2D
{
	private Label currentLevelLabel;

	private GameState gameState;
    //private CustomSignals customSignals;
    
    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        //customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        currentLevelLabel = GetNode<Label>("Label");
	}

    public override void _Process(double delta)
    {
        if (!gameState.IsLevelTransitionPlaying)
        {
            gameState.InGameTime += delta;
        }

        int seconds = (int)gameState.InGameTime;
        int miliseconds = (int)(gameState.InGameTime%1 * 1000);

        TimeSpan span = new(0, 0, seconds);
        currentLevelLabel.Text = string.Format("{0:0}:{1:00}:{2:00}.{3:000}", span.Hours, span.Minutes, span.Seconds, miliseconds);
    }
}
