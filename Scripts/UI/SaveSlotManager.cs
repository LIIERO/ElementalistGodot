using Godot;
using System;
using System.Reflection;

public partial class SaveSlotManager : Control
{
    private enum SelectionMode { disabled, newGamePressed, continuePressed }
    private SelectionMode selectionMode = SelectionMode.disabled; 

    private GameState gameState;
    private AudioManager audioManager;
    private CustomSignals customSignals;
    private LevelTransitions levelTransitions;

    public int CurrentItemIndex { get; private set; }
    //protected int startingIndex = 0;
    [Export] private SaveSlot[] saveSlotList;
    private int maxSlotIndex;

    // Save file override popup
    [Export] private PackedScene yesNoScreen;
    private YesNoScreen newGameApprovalPopup = null;
    private const string newGameApprovalPopupText = "Are you sure you want to reset your save file? All progress will be lost.";

    public override void _Ready()
	{
        Hide();
        selectionMode = SelectionMode.disabled;

        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PopupResult, new Callable(this, MethodName.NewGame));
        customSignals.Connect(CustomSignals.SignalName.SwitchToSelectSaveFileMode, new Callable(this, MethodName.ActivateSaveSlotSelection));

        maxSlotIndex = saveSlotList.Length - 1;
    }

    private void ActivateSaveSlotSelection(bool newGame)
    {
        selectionMode = newGame ? SelectionMode.newGamePressed : SelectionMode.continuePressed;

        // Find save slot index with given ID
        for (int i = 0; i <= maxSlotIndex; i++)
        {
            if (saveSlotList[i].SlotID == gameState.CurrentSaveFileID)
            {
                CurrentItemIndex = i;
                break;
            }
        }

        SelectSlot(CurrentItemIndex);

        Show();
    }

    public override void _Process(double delta)
    {
        if (selectionMode == SelectionMode.disabled) return;

        if (InputManager.UIRightPressed())
        {
            audioManager.buttonSelected.Play();
            DeselectSlot(CurrentItemIndex);
            CurrentItemIndex += 1;
            if (CurrentItemIndex > maxSlotIndex) CurrentItemIndex = 0;
            SelectSlot(CurrentItemIndex);
            //hoverSound.Play();
        }

        if (InputManager.UILeftPressed())
        {
            audioManager.buttonSelected.Play();
            DeselectSlot(CurrentItemIndex);
            CurrentItemIndex -= 1;
            if (CurrentItemIndex < 0) CurrentItemIndex = maxSlotIndex;
            SelectSlot(CurrentItemIndex);
            //hoverSound.Play();
        }

        if (InputManager.UIAcceptPressed())
        {
            string selectedSlotID = saveSlotList[CurrentItemIndex].SlotID;

            if (selectionMode == SelectionMode.newGamePressed)
            {
                if (gameState.SaveFileExists(selectedSlotID)) // Make sure player wants to reset their progress
				{
					newGameApprovalPopup = yesNoScreen.Instantiate() as YesNoScreen;
					AddChild(newGameApprovalPopup);
					newGameApprovalPopup.SetText(newGameApprovalPopupText);
					newGameApprovalPopup.CreatePopup();
				}
				else NewGame(true);
            }

            if (selectionMode == SelectionMode.continuePressed)
            {
                if (!gameState.SaveFileExists(selectedSlotID)) return;
				gameState.ResetPersistentData();
				gameState.LoadFromSaveFile(selectedSlotID);
				levelTransitions.StartGameTransition(); // transition from menu to game
            }
        }

        if (InputManager.UICancelPressed())
        {
            Hide();
            selectionMode = SelectionMode.disabled;

            DeselectSlot(CurrentItemIndex);
            customSignals.EmitSignal(CustomSignals.SignalName.SwitchToNormalMenuMode);
        }
    }

    private void SelectSlot(int index)
    {
        saveSlotList[index].Select();
    }

    private void DeselectSlot(int index)
    {
        saveSlotList[index].Deselect();
    }

    public void ResetButtons()
    {
        DeselectSlot(CurrentItemIndex);
        CurrentItemIndex = 0;
        SelectSlot(CurrentItemIndex);
    }

    private void NewGame(bool areYouSure)
    {
        newGameApprovalPopup = null;

        if (areYouSure == false) return;
        gameState.ResetPersistentData();
        gameState.CreateNewSaveFile(saveSlotList[CurrentItemIndex].SlotID); // Create a new save with the default values
        levelTransitions.StartGameTransition(); // transition from menu to game
    }
}
