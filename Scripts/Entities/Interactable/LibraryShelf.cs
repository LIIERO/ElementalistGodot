using Godot;
using System;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

public partial class LibraryShelf : Interactable
{
    private CustomSignals customSignals;
    private GameState gameState;

    public bool Locked { get; set; } = false;

    [Export] private string lockedText;
    [Export] private string libraryID;

    //private Node2D unlockImages;
    //private Sprite2D lockSprite;
    private Label levelNameDisplay;
    private Label levelNameDisplayShadow;
    private AnimationPlayer nameDisplayAnimation;

    public override void _Ready()
	{
		base._Ready();

        if (Locked) GetNode<Node2D>("Images").Hide();
        else GetNode<Sprite2D>("Lock").Hide();

        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        nameDisplayAnimation = GetNode<AnimationPlayer>("InfoAnimation");
        levelNameDisplay = GetNode<Label>("InfoCard");
        levelNameDisplayShadow = levelNameDisplay.GetNode<Label>("Shadow");
        string libraryName = gameState.LibraryData[libraryID]["name"][0]; // 1 element list
        levelNameDisplay.Text = libraryName;
        levelNameDisplayShadow.Text = libraryName;
        levelNameDisplay.Hide();
    }

    protected override void Interact()
	{
		base.Interact();

        if (Locked)
        {
            customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, lockedText);
            return;
        }

        customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, libraryID);
    }

    protected override void PlayerEntered()
    {
        base.PlayerEntered();
        levelNameDisplay.Show();
        nameDisplayAnimation.Play("Appear");
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();
        nameDisplayAnimation.PlayBackwards("Appear");
        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
    }
}
