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
    public delegate void SetPlayerPositionEventHandler(Vector2 position, bool fireTeleport, float fireballDirection);

    // Camera

    [Signal]
    public delegate void SetCameraPositionEventHandler(Vector2 position);

    [Signal]
    public delegate void ShiftCameraXLimitsEventHandler(int left, int right);

    // UI

    [Signal]
    public delegate void StartDialogEventHandler(string dialogID);

    [Signal]
    public delegate void EndDialogEventHandler();

    [Signal]
    public delegate void ProgressDialogEventHandler();

    [Signal]
    public delegate void PopupResultEventHandler(bool result);

    [Signal]
    public delegate void GamePausedEventHandler(bool paused);

    [Signal]
    public delegate void SwitchToSelectSaveFileModeEventHandler(bool newGame);

    [Signal]
    public delegate void SwitchToNormalMenuModeEventHandler();

    // Gameplay

    [Signal]
    public delegate void ElementChargeActivatedEventHandler();

    [Signal]
    public delegate void LevelTransitionedEventHandler();

    // Undo system

    [Signal]
    public delegate void RequestCheckpointEventHandler();

    [Signal]
    public delegate void AddCheckpointEventHandler();

    [Signal]
    public delegate void ReplaceTopCheckpointEventHandler();

    [Signal]
    public delegate void UndoCheckpointEventHandler(bool nextCheckpointRequested);

}
