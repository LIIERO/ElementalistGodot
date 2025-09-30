using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class PauseMenu : ButtonManager
{
    const int RESUME = 0;
    const int HINTSKIP = 1;
    const int OPTIONS = 2;
    const int EXIT = 3;

    [Export] private PackedScene hintPopup;
    private HintPopup hintApprovalPopup = null;
    private string hintApprovalPopupText;

    // singletons
    //private SettingsManager settingsManager;
    private LevelTransitions levelTransitions;
    private AudioManager audioManager;
    private CustomSignals customSignals;

    const float resumeDelay = 0.1f;

	public override void _Ready()
	{
        //settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.HintPopupResult, new Callable(this, MethodName.DelayResumeAndShowHint));

        base._Ready();

        hintApprovalPopupText = gameState.UITextData["hint_warning"];

        Resume();
	}

    public override void _Process(double delta)
	{
        if (hintApprovalPopup != null) return;

        if (InputManager.PausePressed())
        {
            if (gameState.IsGamePaused)
            {
                DelayResume();
            }
            else if (!gameState.IsLevelTransitionPlaying) // Cant pause during transitions
            {
                ResetButtons();
                Pause();
            }
        }
        else if (InputManager.UICancelPressed() && gameState.IsGamePaused)
            DelayResume();

        if (!gameState.IsGamePaused) return;

		base._Process(delta); // button stuff

        if (InputManager.UIAcceptPressed())
        {
            if (CurrentItemIndex == RESUME)
            {
                DelayResume();
            }
            if (CurrentItemIndex == HINTSKIP)
            {
                // The button works conditionally based on whats happening, the appearance of the button is set in HintSkipMenuButton.cs
                if (gameState.IsDialogActive) // If the dialog is active it skips it
                {
                    DelayResumeAndSkipCutscene();
                }
                else if (!gameState.IsHubLoaded()) // If no dialog and player in a non hub level it shows hint after a warning
                {
                    hintApprovalPopup = hintPopup.Instantiate() as HintPopup;
                    AddChild(hintApprovalPopup);
                    hintApprovalPopup.SetText(hintApprovalPopupText);
                    hintApprovalPopup.CreatePopup();
                }
            }

            if (CurrentItemIndex == OPTIONS)
            {
                gameState.SaveToSaveFile(gameState.CurrentSaveFileID);
                audioManager.StopMusic();
                Resume();
                levelTransitions.StartOptionsTransition();
            }
            if (CurrentItemIndex == EXIT)
            {
                gameState.SaveToSaveFile(gameState.CurrentSaveFileID);
                audioManager.StopMusic();
                Resume();
                MainMenu.sceneEnterItemIndex = 1; // Menu starts with continue selected
                levelTransitions.StartMenuTransition();
            }
            /*if (CurrentItemIndex == 3) // exit game
            {
                audioManager.StopMusic();
                gameState.SaveToSaveFile("0");
                settingsManager.SavePreferences();
                GetTree().Quit();
            }*/
        }
    }

    private void Pause()
	{
        //Engine.TimeScale = 0;
        //customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, true);
        Show();
        GetTree().Paused = true;
        gameState.IsGamePaused = true;
        customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, true);
    }

	private void Resume()
	{
        //Engine.TimeScale = 1;
        Hide();

        GetTree().Paused = false;
        gameState.IsGamePaused = false;
        customSignals.EmitSignal(CustomSignals.SignalName.GamePaused, false);
    }

    public async void DelayResume()
    {
        await ToSignal(GetTree().CreateTimer(resumeDelay), "timeout");
        Resume();
    }

    private async void DelayResumeAndShowHint(bool showHint, int noHint)
    {
        await ToSignal(GetTree().CreateTimer(resumeDelay), "timeout");
        Resume();
        hintApprovalPopup = null;

        if (showHint) customSignals.EmitSignal(CustomSignals.SignalName.StartHintDialog, noHint);
    }

    private async void DelayResumeAndSkipCutscene()
    {
        await ToSignal(GetTree().CreateTimer(resumeDelay), "timeout");
        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
        Resume();
    }
}
