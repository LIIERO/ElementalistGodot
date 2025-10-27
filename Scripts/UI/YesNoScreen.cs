using Godot;
using System;

public partial class YesNoScreen : CanvasLayer
{
    // singletons
    private CustomSignals customSignals;
    protected GameState gameState;
    private AudioManager audioManager;

    [Export] private MenuButton noButton;
    [Export] private MenuButton yesButton;
    [Export] private Label popupText;

    private bool isYesSelected;
    private string id = "";

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
    }

    public void SetText(string text)
    {
        popupText.Text = text;
    }

    public void CreatePopup(string popupId="")
    {
        id = popupId;
        Show();
        GetTree().Paused = true;
        gameState.IsGamePaused = true;
        isYesSelected = false;
        SelectButton();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (InputManager.PausePressed())
        {
            ReturnFromPopup(false);
        }
        else if (InputManager.UILeftPressed() || InputManager.UIRightPressed())
        {
            isYesSelected = !isYesSelected;
            SelectButton();
        }
        else if (InputManager.UIAcceptPressed())
        {
            ReturnFromPopup(isYesSelected);
        }
    }

    private void ReturnFromPopup(bool result)
    {
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
        customSignals.EmitSignal(CustomSignals.SignalName.PopupResult, result, id);
        QueueFree();
    }

    private void SelectButton()
    {
        audioManager.buttonSelected.Play();

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
