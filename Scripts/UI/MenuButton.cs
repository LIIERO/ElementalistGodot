using Godot;
using System;

public partial class MenuButton : Node2D
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

	public void Select()
	{
        orbSelection.Show();
        Position = offsetPosition;
    }

	public void Deselect()
	{
        orbSelection.Hide();
        Position = basePosition;
    }
}
