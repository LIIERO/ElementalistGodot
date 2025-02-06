using Godot;
using System;

public abstract partial class UIInteractable : Node2D
{
	private Node2D orbSelection;
	private Vector2 basePosition;
	private Vector2 offsetPosition;

	public override void _Ready()
	{
		orbSelection = GetNode("./OrbSelection") as Node2D;
		basePosition = Position;
		offsetPosition = basePosition + new Vector2(GameUtils.gameUnitSize, 0.0f);
        orbSelection.Hide();
    }

	public virtual void Select()
	{
        orbSelection.Show();
        Position = offsetPosition;
    }

	public virtual void Deselect()
	{
        orbSelection.Hide();
        Position = basePosition;
    }

	public virtual void SetOpacityToHalf()
	{
        Modulate = new Color(1f, 1f, 1f, 0.5f);
    }

	public virtual void SetOpacityToNormal()
	{
        Modulate = new Color(1f, 1f, 1f, 1f);
    }
}
