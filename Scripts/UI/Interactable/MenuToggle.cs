using Godot;
using System;

public partial class MenuToggle : UIInteractable
{

    public bool Toggled { get; private set; }

    private Node2D toggleCheckmark;

	public override void _Ready()
	{
        toggleCheckmark = GetNode<Node2D>("ToggleCheckmark");

        base._Ready();
    }

	public override void Select()
	{
        base.Select();
    }

	public override void Deselect()
	{
        base.Deselect();
    }

    public void Toggle(bool toggled)
    {
        Toggled = toggled;

        if (Toggled)
            toggleCheckmark.Show();
        else toggleCheckmark.Hide();
    }
}
