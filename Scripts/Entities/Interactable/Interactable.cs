using Godot;
using Godot.Collections;
using System;

public abstract partial class Interactable : Area2D
{
    [Export] public float arrowYOffset = -2.0f;
    [Export] private PackedScene arrowIndicatorPrefab;
    private AnimatedSprite2D arrowIndicator;
    protected Player playerScriptReference;
    public bool IsActive { get; private set; } = false;

    private CustomSignals customSignals; // singleton

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PlayerInteracted, new Callable(this, MethodName.OnPlayerInteract));

        arrowIndicator = arrowIndicatorPrefab.Instantiate() as AnimatedSprite2D;
        AddChild(arrowIndicator);
        arrowIndicator.Position += new Vector2(0.0f, arrowYOffset * GameUtils.gameUnitSize);

        IsActive = false;
        arrowIndicator.Hide();
    }

    private void OnPlayerInteract()
    {
        if (!IsActive) return;
        if (playerScriptReference == null) return;
        if (!playerScriptReference.isGrounded) return;
        Interact();
    }

    void _OnBodyEntered(Node2D body)
    {
        if (body is not Player) return;
        playerScriptReference = body as Player;
        PlayerEntered();
    }

    void _OnBodyExited(Node2D body)
    {
        if (body is not Player) return;
        PlayerExited();
    }

    protected virtual void PlayerEntered()
    {
        IsActive = true;
        arrowIndicator.Show();
        arrowIndicator.Play("Idle");
    }

    protected virtual void PlayerExited()
    {
        IsActive = false;
        arrowIndicator.Hide();
    }

    protected virtual void Interact()
    {

    }
}
