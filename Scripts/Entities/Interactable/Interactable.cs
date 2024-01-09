using Godot;
using Godot.Collections;
using System;

public abstract partial class Interactable : Area2D
{
    [Export] public float arrowYOffset = -2.0f;
    [Export] private PackedScene arrowIndicatorPrefab;
    private AnimatedSprite2D arrowIndicator;
    private Player playerScriptReference;
    public bool IsActive { get; private set; } = false;

    
    public override void _Ready()
	{
        arrowIndicator = arrowIndicatorPrefab.Instantiate() as AnimatedSprite2D;
        AddChild(arrowIndicator);
        arrowIndicator.Position += new Vector2(0.0f, arrowYOffset * GlobalUtils.gameUnitSize);

        IsActive = false;
        arrowIndicator.Hide();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!IsActive) return;
        if (!playerScriptReference.interactPressed) return;

        Interact();
	}

    void _OnBodyEntered(Node2D body)
    {
        if (body is not Player) return;
        PlayerEntered(body);
    }

    void _OnBodyExited(Node2D body)
    {
        if (body is not Player) return;
        PlayerExited();
    }

    protected void PlayerEntered(Node2D player)
    {
        playerScriptReference = player as Player;
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
