using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class Letter : PermanentCollectable
{
    [Export] private string letter;
    private AnimatedSprite2D sprite;

    public override void _Ready()
    {
        base._Ready();
        sprite = GetNode<AnimatedSprite2D>("MovedByAnimation/AnimatedSprite2D");
        sprite.Play(letter);
    }

    protected override void OnCollected()
    {
        gameState.UnlockedLetters.Add(letter);
    }

    protected override bool IsCollected()
    {
        return gameState.UnlockedLetters.Contains(letter);
    }
}
