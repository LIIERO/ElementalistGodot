using Godot;
using System;

public static class InputManager
{
    public static bool IsGamepadConnected { get; set; } = false; // set in gameState every frame


    // Gameplay
	public static float GetLeftRightGameplayDirection()
	{
        if (IsGamepadConnected)
        {
            float axis = Input.GetAxis("inputLeftGamepad", "inputRightGamepad");
            if (axis > 0.01f && axis < 0.5f) axis = 0.5f;
            if (axis < -0.01f && axis > -0.5f) axis = -0.5f;
            if (axis > 0.8f) axis = 1.0f;
            if (axis < -0.8f) axis = -1.0f;
            return axis;
        }   
		return Input.GetAxis("inputLeft", "inputRight");
    }

    public static bool UpInteractPressed()
	{
        if (IsGamepadConnected)
        {
            if (!Input.IsActionJustPressed("inputUpGamepad")) return false;
            float Xaxis = Input.GetAxis("inputLeftGamepad", "inputRightGamepad");
            float Yaxis = Input.GetAxis("inputDownGamepad", "inputUpGamepad");
            return Xaxis == 0f && Yaxis > 0.1f;
        }
        return Input.IsActionJustPressed("inputUp");
    }

    public static bool JumpPressed()
    {
        if (IsGamepadConnected)
            return Input.IsActionJustPressed("inputJumpGamepad");
        return Input.IsActionJustPressed("inputJump");
    }

    public static bool AbilityPressed()
    {
        if (IsGamepadConnected)
            return Input.IsActionJustPressed("inputAbilityGamepad");
        return Input.IsActionJustPressed("inputAbility");
    }

    public static bool RestartPressed()
    {
        if (IsGamepadConnected)
            return Input.IsActionJustPressed("inputRestartGamepad");
        return Input.IsActionJustPressed("inputRestart");
    }

    public static bool JumpReleased()
    {
        if (IsGamepadConnected)
            return Input.IsActionJustReleased("inputJumpGamepad");
        return Input.IsActionJustReleased("inputJump");
    }
    

    // Menus
    public static bool PausePressed()
    {
        if (IsGamepadConnected)
            return Input.IsActionJustPressed("inputPauseGamepad");
        return Input.IsActionJustPressed("inputPause");
    }

    public static bool UIUpPressed()
    {
        return Input.IsActionJustPressed("ui_up");
    }

    public static bool UIDownPressed()
    {
        return Input.IsActionJustPressed("ui_down");
    }

    public static bool UILeftPressed()
    {
        return Input.IsActionJustPressed("ui_left");
    }

    public static bool UIRightPressed()
    {
        return Input.IsActionJustPressed("ui_right");
    }

    public static bool UIAcceptPressed()
    {
        return Input.IsActionJustPressed("ui_accept");
    }

    public static bool UICancelPressed()
    {
        return Input.IsActionJustPressed("ui_cancel");
    }

    public static bool UIGamepadCancelPressed()
    {
        return Input.IsActionJustPressed("ui_cancel_gamepad");
    }
}
