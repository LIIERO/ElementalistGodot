using Godot;
using System;

public partial class LetterDisplay : AnimatedSprite2D
{
    [Export] private string letter = "_";

    public override void _Ready()
	{
        Play(letter);
	}
}
