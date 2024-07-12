using Godot;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Tutorial : Node2D
{
	[Export]
	private string actionName;
    private string gamepadActionName;

    private Node2D keyboardTutorial;
	private AnimatedSprite2D gamepadTutorial;

	public override void _Ready()
	{
        gamepadActionName = actionName + "Gamepad";

        Input.Singleton.Connect("joy_connection_changed", new Callable(this, nameof(OnJoyConnectionChanged)));
        keyboardTutorial = GetNode<Node2D>("KeyboardTutorial");
        gamepadTutorial = GetNode<AnimatedSprite2D>("GamepadButtonDisplay");

        // keyboard prompt
		string keyText = InputMap.ActionGetEvents(actionName)[0].AsText().ToUpper();
		keyboardTutorial.GetNode<Label>("Text").Text = keyText;
		if (keyText.Length > 1) keyboardTutorial.GetNode<Sprite2D>("KeyImage").QueueFree();

        // gamepad prompt
        string gamepadButtonName = (InputMap.ActionGetEvents(gamepadActionName)[0] as InputEventJoypadButton).ButtonIndex.ToString();
        if (gamepadTutorial.SpriteFrames.GetAnimationNames().Contains(gamepadButtonName))
        {
            gamepadTutorial.Play(gamepadButtonName);
        }
        else
        {
            gamepadTutorial.Play("default");
        }


        if (InputManager.IsGamepadConnected)
			keyboardTutorial.Hide();
		else
			gamepadTutorial.Hide();
    }

    private void OnJoyConnectionChanged(int device, bool connected)
    {
        if (connected)
		{
            keyboardTutorial.Hide();
            gamepadTutorial.Show();
        }
        else
        {
            keyboardTutorial.Show();
            gamepadTutorial.Hide();
        }
    }
}
