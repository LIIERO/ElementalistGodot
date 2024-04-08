using Godot;
using System;

public partial class YesNoScreen : CanvasLayer
{
    // singletons
    private CustomSignals customSignals;
    protected GameState gameState;

    [Export] private MenuButton noButton;
    [Export] private MenuButton yesButton;
    [Export] private Label popupText;

    private bool isYesSelected;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
    }

    public void SetText(string text)
    {
        popupText.Text = text;
    }

    public void CreatePopup()
    {
        Show();
        GetTree().Paused = true;
        gameState.IsGamePaused = true;
        isYesSelected = false;
        SelectButton();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("inputPause"))
        {
            ReturnFromPopup(false);
        }
        else if (Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_right"))
        {
            isYesSelected = !isYesSelected;
            SelectButton();
        }
        else if (Input.IsActionJustPressed("ui_accept"))
        {
            ReturnFromPopup(isYesSelected);
        }
    }

    private void ReturnFromPopup(bool result)
    {
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
        customSignals.EmitSignal(CustomSignals.SignalName.PopupResult, result);
        QueueFree();
    }

    private void SelectButton()
    {
        if (isYesSelected)
        {
            yesButton.Select();
            noButton.Deselect();
        }
        else
        {
            yesButton.Deselect();
            noButton.Select();
        }
    }
}
