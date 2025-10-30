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
    public delegate void StartHintDialogEventHandler(int noHint);

    [Signal]
    public delegate void EndDialogEventHandler();

    [Signal]
    public delegate void ProgressDialogEventHandler();

    [Signal]
    public delegate void PopupResultEventHandler(bool result, string popupId);

    [Signal]
    public delegate void HintPopupResultEventHandler(bool result, int noHint);

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
    public delegate void CollectedPermanentEventHandler();

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

    [Signal]
    public delegate void UnlockCheckpointingEventHandler(); // Used by Earth block


    // Purple Forest puzzle

    [Signal]
    public delegate void SwapEarthBlockColorEventHandler();
}
