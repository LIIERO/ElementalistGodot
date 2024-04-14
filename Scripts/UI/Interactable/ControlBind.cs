using Godot;
using System;

public partial class ControlBind : UIInteractable
{
    private Label inputLabel;

    Color white = new(1.0f, 1.0f, 1.0f, 1.0f);
    Color yellow = new(1.0f, 1.0f, 0.0f, 1.0f);

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
        inputLabel.Set("theme_override_colors/font_color", white);
    }

    public void SetRebinding()
    {
        inputLabel.Text = "...WAITING FOR INPUT...";
        inputLabel.Set("theme_override_colors/font_color", yellow);
    }
}
