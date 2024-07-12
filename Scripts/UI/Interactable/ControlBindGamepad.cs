using Godot;
using System;
using System.Linq;

public partial class ControlBindGamepad : UIInteractable
{
    private Label inputLabel;
    private AnimatedSprite2D buttonDisplay;

    Color white = new(1.0f, 1.0f, 1.0f, 1.0f);
    Color yellow = new(1.0f, 1.0f, 0.0f, 1.0f);

    public override void _Ready()
	{
        inputLabel = GetNode<Label>("InputSprite/Text");
        buttonDisplay = GetNode<AnimatedSprite2D>("GamepadButtonDisplay");
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

        if (buttonDisplay.SpriteFrames.GetAnimationNames().Contains(inputName))
        {
            buttonDisplay.Play(inputName);
        }
        else
        {
            buttonDisplay.Play("default");
        }
    }

    public void SetRebinding()
    {
        inputLabel.Text = "...WAITING FOR INPUT...";
        inputLabel.Set("theme_override_colors/font_color", yellow);
    }
}
