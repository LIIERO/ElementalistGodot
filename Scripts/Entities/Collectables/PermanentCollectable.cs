using Godot;
using System;

public abstract partial class PermanentCollectable : Area2D
{
    private AudioManager audioManager;
    private CustomSignals customSignals;
    protected GameState gameState;

    public override void _Ready()
    {
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        gameState = GetNode<GameState>("/root/GameState");

        if (IsCollected())
        {
            Hide();
            QueueFree();
            return;
        }
    }

    void _OnBodyEntered(Node2D body)
    {
        if (body is not Player) return;

        //Player playerScript = body as Player;
        OnCollected();
        audioManager.permanentCollectSound.Play();
        Hide();
        QueueFree();
        customSignals.EmitSignal(CustomSignals.SignalName.CollectedPermanent);
    }


    protected abstract void OnCollected();
    protected abstract bool IsCollected();

}
