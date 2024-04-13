using GlobalTypes;
using Godot;
using Godot.Collections;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class InputOptions : ButtonManager
{
    // Button indexes
    private const int BACK = 0;


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
        InputMap.LoadFromProjectSettings();
        for (int i = 1; i < buttonList.Length; i++)
        {
            ControlBind bindButton = buttonList[i] as ControlBind;
            var inputEvent = InputMap.ActionGetEvents(elementIdToAction[i])[0];
            bindButton.SetInputName(inputEvent.AsText().ToUpper());
        }
    }


    public override void _Process(double delta)
	{
        if (gameState.IsLevelTransitionPlaying) return;

		base._Process(delta);

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            gameState.LoadOptions();
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (CurrentItemIndex == BACK)
            {
                gameState.LoadOptions();
            }

            else
            {

            }
        }
    }

    async void InitializeElementsEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");

        SetActionList();
    }
}
