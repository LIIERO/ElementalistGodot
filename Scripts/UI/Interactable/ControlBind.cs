using Godot;
using System;

public partial class ControlBind : UIInteractable
{
    private Label inputLabel;

	public override void _Ready()
	{
        inputLabel = GetNode<Label>("InputSprite/Text");
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

    public void SetInputName(string inputName)
    {
        inputLabel.Text = inputName;
    }
}
