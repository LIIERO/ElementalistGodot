using Godot;
using System;

public partial class HintPopup : CanvasLayer
{
    // singletons
    private CustomSignals customSignals;
    protected GameState gameState;
    private AudioManager audioManager;

    [Export] private MenuButton[] buttonList;
    [Export] private Label popupText;

    public int CurrentItemIndex { get; private set; }
    protected int startingIndex = 0;
    private int maxButtonIndex;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        maxButtonIndex = buttonList.Length - 1;
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
        CurrentItemIndex = startingIndex;
        SelectButton(CurrentItemIndex);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // TODO: Escape to return from popup without a decision
        if (InputManager.PausePressed())
        {
            ReturnFromPopup(false, 0);
        }
        else if (InputManager.UIAcceptPressed())
        {
            bool isHintSelected = !(CurrentItemIndex == startingIndex);
            ReturnFromPopup(isHintSelected, CurrentItemIndex);
        }

        else if (InputManager.UIRightPressed())
        {
            audioManager.buttonSelected.Play();
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex += 1;
            if (CurrentItemIndex > maxButtonIndex) CurrentItemIndex = 0;
            SelectButton(CurrentItemIndex);
        }

        else if (InputManager.UILeftPressed())
        {
            audioManager.buttonSelected.Play();
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex -= 1;
            if (CurrentItemIndex < 0) CurrentItemIndex = maxButtonIndex;
            SelectButton(CurrentItemIndex);
        }
    }

    private void ReturnFromPopup(bool result, int noHint)
    {
        GetTree().Paused = false;
        gameState.IsGamePaused = false;
        customSignals.EmitSignal(CustomSignals.SignalName.HintPopupResult, result, noHint);
        QueueFree();
    }

    protected void SelectButton(int index)
    {
        buttonList[index].Select();
    }

    protected void DeselectButton(int index)
    {
        buttonList[index].Deselect();
    }
}
