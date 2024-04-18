using Godot;
using System;

public partial class ChargeGate : Node2D
{
    private CustomSignals customSignals; // Singleton
    private AudioManager audioManager;
    private AnimationPlayer animator;
    private Label requiredFragmentsDisplay;

    private bool isOpened = false;
    [Export] private int requiredCharges;
    private int noCharges = 0;

    public override void _Ready()
	{
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.ElementChargeActivated, new Callable(this, MethodName.AddCharge));

        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        requiredFragmentsDisplay = GetNode<Label>("ToMove/Text/Label");
        requiredFragmentsDisplay.Text = requiredCharges.ToString();
    }

    private void AddCharge()
    {
        noCharges++;

        if (!isOpened && noCharges >= requiredCharges)
        {
            audioManager.gateOpen.Play();
            isOpened = true;
            animator.Play("Open");
        }
    }
}
