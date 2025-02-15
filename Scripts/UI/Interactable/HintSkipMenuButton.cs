using Godot;
using System;

public partial class HintSkipMenuButton : MenuButton
{
    //private enum State { inactive, skipDialog, hint }
    //private State currentState = State.inactive;

    [Export] private string skipTextID;
    [Export] private string hintTextID;

    private CustomSignals customSignals; // Singleton

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.GamePaused, new Callable(this, MethodName.SetAppearance));

        base._Ready();

        SetOpacityToHalf();
    }

    private void SetAppearance(bool paused)
    {
        if (!paused) return;

        if (gameState.IsDialogActive)
        {
            interactableLabel.Text = gameState.UITextData["skip_cutscene"];
            SetModulateToYellow();
            SetOpacityToNormal();
        }
        else if (!gameState.IsHubLoaded())
        {
            interactableLabel.Text = gameState.UITextData["hint"];
            SetModulateToYellow();
            SetOpacityToNormal();
        }
        else
        {
            interactableLabel.Text = "";
            SetModulateToWhite();
            SetOpacityToHalf();
        }
    }

	public override void Select()
	{
        base.Select();
    }

	public override void Deselect()
	{
        base.Deselect();
    }
}
