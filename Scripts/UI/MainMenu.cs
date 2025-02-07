using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class MainMenu : ButtonManager
{
	private bool saveSlotSelectionActive = false;

    private const int NEWGAME = 0;
	private const int CONTINUE = 1;
	private const int OPTIONS = 2;
	private const int EXIT = 3;
	private const int CREDITS = 4;
	private const int WISHLIST = 5;

	public static int sceneEnterItemIndex = 0; // which button is selected upon entering menu, set before switching to menu

	// Singletons
	private CustomSignals customSignals;
	private LevelTransitions levelTransitions;
	private SettingsManager settingsManager;

	public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.SwitchToNormalMenuMode, new Callable(this, MethodName.DisableSaveSlotSelection));

        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
		gameState = GetNode<GameState>("/root/GameState");

		if (sceneEnterItemIndex == NEWGAME && !gameState.NoSaveFileExists()) // If there is a save file, the "continue" button will be selected first
			sceneEnterItemIndex = CONTINUE;

		startingIndex = sceneEnterItemIndex;
		base._Ready();

        if (gameState.NoSaveFileExists()) buttonList[CONTINUE].SetOpacityToHalf();

    }

	public override void _Process(double delta)
	{
		if (gameState.IsLevelTransitionPlaying || saveSlotSelectionActive) return;

		base._Process(delta);

		if (InputManager.UIAcceptPressed())
		{
			if (CurrentItemIndex == NEWGAME)
			{
				DeselectButton(CurrentItemIndex);
                saveSlotSelectionActive = true;
				SwitchToSaveSlotModeEndFrame(true); // new game = true
            }
			if (CurrentItemIndex == CONTINUE)
			{
				if (gameState.NoSaveFileExists()) return;

                DeselectButton(CurrentItemIndex);
                saveSlotSelectionActive = true;
                SwitchToSaveSlotModeEndFrame(false); // new game = false
            }
			if (CurrentItemIndex == OPTIONS)
			{
				levelTransitions.StartOptionsTransition();
			}
			if (CurrentItemIndex == EXIT)
			{
				settingsManager.SavePreferences();
				GetTree().Quit();
			}

			if (CurrentItemIndex == CREDITS)
			{
				levelTransitions.StartCreditsTransition();
			}
			if (CurrentItemIndex == WISHLIST)
			{
				OS.ShellOpen("https://store.steampowered.com/app/3029010/Elementalist/");
			}
		}
	}

    async void SwitchToSaveSlotModeEndFrame(bool newGame)
    {
        await ToSignal(GetTree(), "process_frame");
        customSignals.EmitSignal(CustomSignals.SignalName.SwitchToSelectSaveFileMode, newGame); // Handled in SaveSlotManager
    }

    private void DisableSaveSlotSelection()
	{
        SelectButton(CurrentItemIndex);
        saveSlotSelectionActive = false;
    }

}
