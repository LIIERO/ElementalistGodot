using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Tutorial : Node2D
{
	[Export]
	private string actionName;

    private Node2D keyboardTutorial;
	private Node2D gamepadTutorial;

	public override void _Ready()
	{
		
		keyboardTutorial = GetNode<Node2D>("KeyboardTutorial");
        gamepadTutorial = GetNode<Node2D>("GamepadTutorial");

		string keyText = InputMap.ActionGetEvents(actionName)[0].AsText().ToUpper();
		keyboardTutorial.GetNode<Label>("Text").Text = keyText;
		if (keyText.Length > 1) keyboardTutorial.GetNode<Sprite2D>("KeyImage").QueueFree();


        if (InputManager.IsGamepadConnected)
			keyboardTutorial.QueueFree();
		else
			gamepadTutorial.QueueFree();
    }
}
