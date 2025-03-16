using Godot;
using System;

public abstract partial class UIInteractable : Node2D
{
	[Export] private string interactableTextID = "";
	protected Label interactableLabel;

	private Node2D orbSelection;
	private Vector2 basePosition;
	private Vector2 offsetPosition;

    protected GameState gameState; // Singleton

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        interactableLabel = GetNode<Label>("Sprite2D/Text");

        try
        {
            interactableLabel.Text = gameState.UITextData[interactableTextID];
        }
        catch
        {
            interactableLabel.Text = interactableTextID;
        }

        orbSelection = GetNode<Node2D>("./OrbSelection");
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

	public virtual void SetModulateToYellow()
	{
        Modulate = new Color(1.0f, 1.0f, 0.0f, Modulate.A);
    }

    public virtual void SetModulateToWhite()
    {
        Modulate = new Color(1.0f, 1.0f, 1.0f, Modulate.A);
    }

    public virtual void SetOpacityToHalf()
	{
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.5f);
    }

	public virtual void SetOpacityToNormal()
	{
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
    }
}
