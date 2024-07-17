using GlobalTypes;
using Godot;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class InputOptionsGamepad : ButtonManager
{
    private const int BACK = 0;
    private const int RESTOREDEFAULT = 6;

    private bool isRemapping = false;
    private bool isWaitingForInput = false;
    private string actionToRemap = null;

    // Singletons
    private LevelTransitions levelTransitions;
    private SettingsManager settingsManager;

    private Dictionary<int, string> elementIdToAction = new() {
        //{ 1, "inputLeftGamepadSecond" },
        //{ 2, "inputRightGamepadSecond" },
        { 1, "inputUpGamepadSecond" },
        { 2, "inputJumpGamepad" },
        { 3, "inputAbilityGamepad" },
        { 4, "inputRestartGamepad" },
        { 5, "inputPauseGamepad" }};

    public override void _Ready()
	{
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
        base._Ready();
        InitializeElementsEndFrame();
	}

    private void SetActionList()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            if (buttonList[i] is not ControlBindGamepad) continue;
            (buttonList[i] as ControlBindGamepad).SetInputName((InputMap.ActionGetEvents(elementIdToAction[i])[0] as InputEventJoypadButton).ButtonIndex.ToString());
        }
    }

    private void GoBack()
    {
        settingsManager.SaveKeybinds(CreateKeybindData(), gamepad:true);
        gameState.LoadOptions();
    }


    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;
        if (InputManager.UIKeyboardCancelPressed()) GoBack(); // if someone is only using keyboard and tries to change gamepad settings, so they can get out
        if (isRemapping) return;

        base._Process(delta);

        if (InputManager.UICancelPressed()) GoBack();

        else if (InputManager.UIAcceptPressed())
        {
            if (CurrentItemIndex == BACK) GoBack();

            else if (CurrentItemIndex == RESTOREDEFAULT)
            {
                InputMap.LoadFromProjectSettings();
                settingsManager.LoadKeyboardKeybinds(); // we dont wanna erase them
                SetActionList();
            }

            else // The rest of the buttons are control binds for input remapping
            {
                isRemapping = true;
                isWaitingForInput = true;
                actionToRemap = elementIdToAction[CurrentItemIndex];
                (buttonList[CurrentItemIndex] as ControlBindGamepad).SetRebinding();
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!isRemapping) return;
        if (!isWaitingForInput) return;

        if (@event is InputEventJoypadButton buttonEvent && buttonEvent.Pressed)
        {
            InputMap.ActionEraseEvents(actionToRemap);
            InputMap.ActionAddEvent(actionToRemap, buttonEvent);
            (buttonList[CurrentItemIndex] as ControlBindGamepad).SetInputName(buttonEvent.ButtonIndex.ToString());
            SetRemappingFalseAfterDelay();
            isWaitingForInput = false;
            actionToRemap = null;
            AcceptEvent();
        }
    }

    private Dictionary<string, byte[]> CreateKeybindData()
    {
        Dictionary<string, byte[]> keybindData = new();

        foreach (string action in elementIdToAction.Values)
        {    
            var inputEvent = InputMap.ActionGetEvents(action)[0];
            keybindData.Add(action, GD.VarToBytesWithObjects(inputEvent as InputEventJoypadButton));
            
        }
        return keybindData;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            settingsManager.SaveKeybinds(CreateKeybindData(), gamepad: true);
        }
    }

    async void InitializeElementsEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");

        SetActionList();
    }

    async void SetRemappingFalseAfterDelay()
    {
        await ToSignal(GetTree().CreateTimer(0.1f, processInPhysics: true), "timeout");

        isRemapping = false;
    }
}
