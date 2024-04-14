using GlobalTypes;
using Godot;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class InputOptions : ButtonManager
{
    private const int BACK = 0;
    private const int RESTOREDEFAULT = 8;

    private bool isRemapping = false;
    private bool isWaitingForInput = false;
    private string actionToRemap = null;

    // Singletons
    private LevelTransitions levelTransitions;
    private SettingsManager settingsManager;

    private Dictionary<int, string> elementIdToAction = new() { 
        { 1, "inputLeft" }, 
        { 2, "inputRight" }, 
        { 3, "inputUp" }, 
        { 4, "inputJump" },
        { 5, "inputAbility" },
        { 6, "inputRestart" },
        { 7, "inputPause" }};

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
            if (buttonList[i] is not ControlBind) continue;
            var inputEvent = InputMap.ActionGetEvents(elementIdToAction[i])[0];
            (buttonList[i] as ControlBind).SetInputName(inputEvent.AsText().ToUpper());
        }
    }

    private void GoBack()
    {
        settingsManager.SaveKeybinds(CreateKeybindData());
        gameState.LoadOptions();
    }


    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;
        if (isRemapping) return;

        base._Process(delta);

        if (Input.IsActionJustPressed("ui_cancel")) GoBack();
        

        else if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == BACK) GoBack();

            else if (CurrentItemIndex == RESTOREDEFAULT)
            {
                InputMap.LoadFromProjectSettings();
                SetActionList();
            }

            else // The rest of the buttons are control binds for input remapping
            {
                isRemapping = true;
                isWaitingForInput = true;
                actionToRemap = elementIdToAction[CurrentItemIndex];
                (buttonList[CurrentItemIndex] as ControlBind).SetRebinding();
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!isRemapping) return;
        if (!isWaitingForInput) return;

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            InputMap.ActionEraseEvents(actionToRemap);
            InputMap.ActionAddEvent(actionToRemap, keyEvent);
            (buttonList[CurrentItemIndex] as ControlBind).SetInputName(keyEvent.Keycode.ToString().ToUpper());
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
            keybindData.Add(action, GD.VarToBytesWithObjects(InputMap.ActionGetEvents(action)[0] as InputEventKey));
        }
        return keybindData;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            settingsManager.SaveKeybinds(CreateKeybindData());
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
