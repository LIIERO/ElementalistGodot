using GlobalTypes;
using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Options : ButtonManager
{
    // Button indexes
    private const int BACK = 0;
    private const int FULLSCREEN = 1;
    private const int RESOLUTION = 2;
    private const int SOUNDVOLUME = 3;
    private const int MUSICVOLUME = 4;
    private const int LIGHTPARTICLES = 5;
    private const int TIMER = 6;
    private const int INPUTSETTINGS = 7;
    private const int GAMEPADINPUTSETTINGS = 8;

    // Singletons
    private LevelTransitions levelTransitions;
    private SettingsManager settingsManager;

    public override void _Ready()
	{
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        base._Ready();
        InitializeElementsEndFrame();
	}

    private void GoBack()
    {
        if (gameState.IsGameplayActive)
        {
            levelTransitions.StartGameTransition();
        }
        else
        {
            MainMenu.sceneEnterItemIndex = 2; // menu starts with options selected
            levelTransitions.StartMenuTransition();
        }
    }

    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;

		base._Process(delta);

        if (InputManager.UICancelPressed())
        {
            GoBack();
        }

        else if (InputManager.UIAcceptPressed())
        {
            if (CurrentItemIndex == BACK)
            {
                GoBack();
            }
            else if (CurrentItemIndex == INPUTSETTINGS)
            {
                gameState.LoadInputOptions();
            }
            else if (CurrentItemIndex == GAMEPADINPUTSETTINGS)
            {
                gameState.LoadGamepadInputOptions();
            }
            else if (CurrentItemIndex == FULLSCREEN) // toggle fullscreen
            {
                if (settingsManager.Fullscreen) {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(false);
                    (buttonList[RESOLUTION] as MenuSelection).Enable();
                    settingsManager.ChangeToWindowed();
                }
                else {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(true);
                    (buttonList[RESOLUTION] as MenuSelection).Disable();
                    settingsManager.ChangeToFullscreen();
                }
            }
            
            else if (CurrentItemIndex == LIGHTPARTICLES) // toggle light and particles
            {
                if (settingsManager.LightParticlesActive) {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(false);
                    settingsManager.SetLightAndParticlesVisibility(false);
                }
                else {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(true);
                    settingsManager.SetLightAndParticlesVisibility(true);
                }
            }

            else if (CurrentItemIndex == TIMER) // toggle wallslide slowdown
            {
                if (settingsManager.SpeedrunTimerVisible) {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(false);
                    settingsManager.SetTimerVisibility(false);
                }
                else {
                    (buttonList[CurrentItemIndex] as MenuToggle).Toggle(true);
                    settingsManager.SetTimerVisibility(true);
                }
            }
        }


        // Move right or left for selections
        if (buttonList[CurrentItemIndex] is not MenuSelection) return;

        if (InputManager.UIRightPressed())
        {
            MenuSelection selection = buttonList[CurrentItemIndex] as MenuSelection;
            bool moved = selection.MoveRight();
            if (!moved) return;

            if (CurrentItemIndex == RESOLUTION) // choose resolution
            {
                settingsManager.ChangeWindowScale(selection.CurrentValueIndex + 1);
            }
            else if (CurrentItemIndex == SOUNDVOLUME) // change sound volume
            {
                settingsManager.ChangeSoundVolume(selection.CurrentValueIndex);
            }
            else if (CurrentItemIndex == MUSICVOLUME) // change music volume
            {
                settingsManager.ChangeMusicVolume(selection.CurrentValueIndex);
            }
        }

        else if (InputManager.UILeftPressed())
        {
            MenuSelection selection = buttonList[CurrentItemIndex] as MenuSelection;
            bool moved = selection.MoveLeft();
            if (!moved) return;

            if (CurrentItemIndex == RESOLUTION) // choose resolution
            {
                settingsManager.ChangeWindowScale(selection.CurrentValueIndex + 1);
            }
            else if (CurrentItemIndex == SOUNDVOLUME) // change sound volume
            {
                settingsManager.ChangeSoundVolume(selection.CurrentValueIndex);
            }
            else if (CurrentItemIndex == MUSICVOLUME) // change music volume
            {
                settingsManager.ChangeMusicVolume(selection.CurrentValueIndex);
            }
        }
    }

    async void InitializeElementsEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");

        (buttonList[FULLSCREEN] as MenuToggle).Toggle(settingsManager.Fullscreen);
        if (settingsManager.Fullscreen) (buttonList[RESOLUTION] as MenuSelection).Disable();
        (buttonList[RESOLUTION] as MenuSelection).SetCurrentValueIndex(settingsManager.WindowScale - 1); // Set to current resoluton option
        (buttonList[SOUNDVOLUME] as MenuSelection).SetCurrentValueIndex(settingsManager.SoundVolume);
        (buttonList[MUSICVOLUME] as MenuSelection).SetCurrentValueIndex(settingsManager.MusicVolume);
        (buttonList[LIGHTPARTICLES] as MenuToggle).Toggle(settingsManager.LightParticlesActive);
        (buttonList[TIMER] as MenuToggle).Toggle(settingsManager.SpeedrunTimerVisible);
    }
}
