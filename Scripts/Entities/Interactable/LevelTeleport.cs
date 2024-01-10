using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class LevelTeleport : Interactable
{
    [Export] private AnimatedSprite2D currentSprite;

    public override void _Ready()
    {
        base._Ready();

        currentSprite.Play("LevelNotCompleted");
    }

    protected override void Interact()
    {
        base.Interact();

        GD.Print("Teleport");
    }
}
