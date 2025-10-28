using Godot;
using System;
using System.Linq;

public partial class CombinationLockConfirm : Interactable
{
    private AnimatedSprite2D checkmark;
    [Export] private CombinationLockInput[] inputs;
    [Export] private EmptyGate gate;

    public override void _Ready()
	{
		base._Ready();
        checkmark = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        HideDisplay();
    }

    protected override void Interact()
	{
		base.Interact();

        if (inputs.All(e => e.NumbersMatching()))
        {
            checkmark.Play("Correct");
            gate.Unlock();
        }
        else
        {
            checkmark.Play("Incorrect");
        }

        checkmark.Show();
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();
    }

    public void HideDisplay()
    {
        checkmark.Hide();
    }
}
