using Godot;
using System;

public static class InputManager
{
    public static bool IsGamepadConnected { get; set; } = false; // set in gameState


    // Gameplay
	public static float GetLeftRightGameplayDirection()
	{
        if (!IsGamepadConnected)
            return Input.GetAxis("inputLeft", "inputRight");

        float analogAxis = Input.GetAxis("inputLeftGamepad", "inputRightGamepad");
        if (analogAxis == 0.0f) return Input.GetAxis("inputLeft", "inputRight");
        else
        {
            if (analogAxis > 0.01f && analogAxis < 0.5f) analogAxis = 0.5f;
            if (analogAxis < -0.01f && analogAxis > -0.5f) analogAxis = -0.5f;
            if (analogAxis > 0.8f) analogAxis = 1.0f;
            if (analogAxis < -0.8f) analogAxis = -1.0f;
            return analogAxis;
        }
    }

    public static bool UpInteractPressed()
	{
        if (Input.IsActionJustPressed("inputUp") || Input.IsActionJustPressed("inputUpGamepadSecond"))
            return true;

        if (!Input.IsActionJustPressed("inputUpGamepad")) return false;
        float Xaxis = Input.GetAxis("inputLeftGamepad", "inputRightGamepad");
        float Yaxis = Input.GetAxis("inputDownGamepad", "inputUpGamepad");
        return Xaxis == 0f && Yaxis > 0.0f;
    }

    public static bool JumpPressed()
    {
        return Input.IsActionJustPressed("inputJump") || Input.IsActionJustPressed("inputJumpGamepad");
    }

    public static bool AbilityPressed()
    {
        return Input.IsActionJustPressed("inputAbility") || Input.IsActionJustPressed("inputAbilityGamepad");
    }

    public static bool RestartPressed()
    {
        return Input.IsActionJustPressed("inputRestart") || Input.IsActionJustPressed("inputRestartGamepad");
    }

    public static bool JumpReleased()
    {
        return Input.IsActionJustReleased("inputJump") || Input.IsActionJustReleased("inputJumpGamepad");
    }
    

    // Menus
    public static bool PausePressed()
    {
        return Input.IsActionJustPressed("inputPause") || Input.IsActionJustPressed("inputPauseGamepad");
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

    public static bool UIKeyboardCancelPressed()
    {
        return Input.IsActionJustPressed("ui_cancel_keyboard");
    }
}
