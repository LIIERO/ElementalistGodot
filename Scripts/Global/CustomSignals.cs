using Godot;
using System;
using GlobalTypes;
using System.Collections.Generic;

public partial class CustomSignals : Node
{
    // Player
    [Signal]
    public delegate void PlayerInteractedEventHandler();

    [Signal]
    public delegate void PlayerDiedEventHandler();

    [Signal]
    public delegate void PlayerAbilityUsedEventHandler(int ability);

    [Signal]
    public delegate void PlayerAbilityListUpdatedEventHandler(int[] abilityArray);

    [Signal]
    public delegate void SetPlayerPositionEventHandler(Vector2 position);

    // Camera

    [Signal]
    public delegate void SetCameraPositionEventHandler(Vector2 position);

    [Signal]
    public delegate void ShiftCameraXLimitsEventHandler(int left, int right);

    // UI

    [Signal]
    public delegate void DialogBoxShowEventHandler(string text); // Much later gonna probably add other stuff like dialog box style or something

    [Signal]
    public delegate void DialogBoxHideEventHandler();

    [Signal]
    public delegate void PopupResultEventHandler(bool result);

    // Gameplay

    [Signal]
    public delegate void ElementChargeActivatedEventHandler();

}
