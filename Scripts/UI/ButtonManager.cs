using Godot;
using System;

public abstract partial class ButtonManager : Panel
{
    protected GameState gameState;

    public int CurrentButtonIndex { get; private set; }
    [Export] protected Node2D[] buttonList;
    //[SerializeField] AudioSource hoverSound;

    int maxButtonIndex;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");

        maxButtonIndex = buttonList.Length - 1;
        CurrentButtonIndex = 0;
        SelectButtonEndFrame(CurrentButtonIndex); // TODO at the end of the frame
    }

    public override void _Process(double delta)
    {
        //if (!buttonList[0].activeInHierarchy) return;

        if (Input.IsActionJustPressed("inputDown"))
        {
            DeselectButton(CurrentButtonIndex);
            CurrentButtonIndex += 1;
            if (CurrentButtonIndex > maxButtonIndex) CurrentButtonIndex = 0;
            SelectButton(CurrentButtonIndex);
            //hoverSound.Play();
        }

        if (Input.IsActionJustPressed("inputUp"))
        {
            DeselectButton(CurrentButtonIndex);
            CurrentButtonIndex -= 1;
            if (CurrentButtonIndex < 0) CurrentButtonIndex = maxButtonIndex;
            SelectButton(CurrentButtonIndex);
            //hoverSound.Play();
        }
    }

    void SelectButton(int index)
    {
        (buttonList[index] as MenuButton).Select();
    }

    void DeselectButton(int index)
    {
        (buttonList[index] as MenuButton).Deselect();
    }

    async void SelectButtonEndFrame(int index)
    {
        await ToSignal(GetTree(), "process_frame");
        SelectButton(index);
    }

    public void ResetButtons()
    {
        DeselectButton(CurrentButtonIndex);
        CurrentButtonIndex = 0;
        SelectButton(CurrentButtonIndex);
    }
}
