using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class MainMenu : ButtonManager
{
    public static int sceneEnterItemIndex = 0; // which button is selected upon entering menu, set before switching to menu

    // Singletons
    private CustomSignals customSignals;
    private LevelTransitions levelTransitions;
    private SettingsManager settingsManager;
    //private CustomSignals customSignals;

    [Export] private PackedScene yesNoScreen;
    private YesNoScreen newGameApprovalPopup = null;
    private const string newGameApprovalPopupText = "Are you sure you want to reset your save file? All progress will be lost.";

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PopupResult, new Callable(this, MethodName.NewGame));

        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        gameState = GetNode<GameState>("/root/GameState");

        if (sceneEnterItemIndex == 0 && gameState.SaveFileExists("0")) // If there is a save file, the "continue" button will be selected first
            sceneEnterItemIndex = 1;

        startingIndex = sceneEnterItemIndex;
		base._Ready();
	}

    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;

        base._Process(delta);

        if (InputManager.UIAcceptPressed())
        {
            if (CurrentItemIndex == 0) // new game
            {
                if (gameState.SaveFileExists("0")) // Make sure player wants to reset their progress
                {
                    newGameApprovalPopup = yesNoScreen.Instantiate() as YesNoScreen;
                    AddChild(newGameApprovalPopup);
                    newGameApprovalPopup.SetText(newGameApprovalPopupText);
                    newGameApprovalPopup.CreatePopup();
                }
                else NewGame(true);
            }
            if (CurrentItemIndex == 1) // continue
            {
                if (!gameState.SaveFileExists("0")) return;
                gameState.ResetPersistentData();
                gameState.LoadFromSaveFile("0");
                levelTransitions.StartGameTransition(); // transition from menu to game
            }
            if (CurrentItemIndex == 2) // options
            {
                levelTransitions.StartOptionsTransition();
            }
            if (CurrentItemIndex == 3) // exit
            {
                settingsManager.SavePreferences();
                GetTree().Quit();
            }

            if (CurrentItemIndex == 4) // credits
            {
                levelTransitions.StartCreditsTransition();
            }
            if (CurrentItemIndex == 5) // wishlist
            {
                OS.ShellOpen("https://store.steampowered.com/app/3029010/Elementalist/");
            }
        }
    }

    private void NewGame(bool areYouSure)
    {
        newGameApprovalPopup = null;

        if (areYouSure == false) return;
        gameState.ResetPersistentData();
        gameState.CreateNewSaveFile("0"); // Create a new save with the default values
        levelTransitions.StartGameTransition(); // transition from menu to game
    }
}
