using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityGate : Gate, IUndoable
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
        IsMovingUp = false;
        gateSprite.FlipV = true;
        animator.Play("Open", customSpeed: openAnimationSpeed);
    }

    protected void Close()
    {
        if (!gameState.IsLevelTransitionPlaying) audioManager.gateOpen.Play();
        isOpened = false;
        IsMovingUp = true; // When it closes it moves back up
        gateSprite.FlipV = false;
        animator.Play("Open", customSpeed: -openAnimationSpeed, fromEnd:true);
    }

    protected override void Reset()
    {
        base.Reset();
        gateSprite.FlipV = false;
    }

    protected void SetOpen()
    {
        isOpened = true;
        IsMovingUp = false;
        gateSprite.FlipV = true;
        animator.Play("SetOpen");
    }

    public override void UndoLocalCheckpoint(bool nextCpRequested)
    {
        base.UndoLocalCheckpoint(nextCpRequested);

        if (isOpened) SetOpen();
    }
}
