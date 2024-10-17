using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityGate : Gate
{

    public override void _Ready()
	{
        base._Ready();

        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityUsed, new Callable(this, MethodName.Toggle));
    }

    private void Toggle(int _) // No matter which ability, the gate will toggle its state
    {
        if (isOpened)
            Close();
        else
            Open();
    }

    protected override void Open()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = true;
        IsAnyGateMovingPrev = true;
        gateSprite.FlipV = true;
        animator.Play("Open", customSpeed: 0.8f);
    }

    protected void Close()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = false;
        IsAnyGateMovingPrev = true;
        gateSprite.FlipV = false;
        animator.Play("Open", customSpeed: -0.8f, fromEnd:true);
    }

    protected override void Reset()
    {
        base.Reset();
        gateSprite.FlipV = false;
    }

    protected void SetOpen()
    {
        isOpened = true;
        IsAnyGateMovingPrev = false;
        gateSprite.FlipV = true;
        animator.Play("SetOpen");
    }

    protected override void AddLocalCheckpoint()
    {
        base.AddLocalCheckpoint();
    }

    protected override void UndoLocalCheckpoint(bool nextCpRequested)
    {
        base.UndoLocalCheckpoint(nextCpRequested);

        if (isOpened) SetOpen();
    }
}
