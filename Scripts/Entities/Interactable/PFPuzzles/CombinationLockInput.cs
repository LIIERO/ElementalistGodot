using Godot;
using System;

public partial class CombinationLockInput : Interactable
{
    private AudioManager audioManager;
    private Label numberDisplay;
    [Export] private CombinationLockConfirm confirm;
    [Export] private int requiredNumber;
    private int currentNumber = 0;

    public override void _Ready()
	{
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        base._Ready();
        numberDisplay = GetNode<Label>("Label");

        numberDisplay.Text = currentNumber.ToString();
    }

    protected override void Interact()
	{
		base.Interact();

        currentNumber++;
        currentNumber %= 10;
        numberDisplay.Text = currentNumber.ToString();
        audioManager.buttonSelected.Play();

        confirm.HideDisplay();
    }

    public bool NumbersMatching()
    {
        return requiredNumber == currentNumber;
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();
    }
}
