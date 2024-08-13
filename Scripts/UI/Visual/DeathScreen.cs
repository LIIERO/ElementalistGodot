using Godot;
using System;

public partial class DeathScreen : Control
{
    // Singletons
    private CustomSignals customSignals;
    
    public override void _Ready()
	{
        Hide();
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PlayerDied, new Callable(this, MethodName.ShowDeathScreen));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.HideDeathScreen));
    }

    private void ShowDeathScreen()
    {
        Show();
    }

    private void HideDeathScreen(bool _)
    {
        Hide();
    }
}
