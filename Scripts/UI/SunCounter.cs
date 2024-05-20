using Godot;
using System;

public partial class SunCounter : Sprite2D
{
    [Export] private bool specialCounter = false;
	[Export] private Label counterText;

    private GameState gameState;
    private CustomSignals customSignals;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.GamePaused, new Callable(this, MethodName.OnPauseAndResume));

        if (!gameState.IsHubLoaded()) Hide();

        if (specialCounter)
            counterText.Text = gameState.NoRedFragments.ToString();
        else
            counterText.Text = gameState.NoSunFragments.ToString();
    }

    void OnPauseAndResume(bool pause)
    {
        if (pause)
            Show();
        else if (!gameState.IsHubLoaded())
            Hide();
    }
}
