using Godot;
using System;

public abstract partial class ButtonManager : Panel
{
    protected GameState gameState;

    public int CurrentItemIndex { get; private set; }
    [Export] protected Node2D[] buttonList; // not only buttons but all interactable stuff from menus like toggles
    //[SerializeField] AudioSource hoverSound;

    int maxButtonIndex;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");

        maxButtonIndex = buttonList.Length - 1;
        CurrentItemIndex = 0;
        SelectButtonEndFrame(CurrentItemIndex); // TODO at the end of the frame
    }

    public override void _Process(double delta)
    {
        //if (!buttonList[0].activeInHierarchy) return;

        if (Input.IsActionJustPressed("ui_down"))
        {
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex += 1;
            if (CurrentItemIndex > maxButtonIndex) CurrentItemIndex = 0;
            SelectButton(CurrentItemIndex);
            //hoverSound.Play();
        }

        if (Input.IsActionJustPressed("ui_up"))
        {
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex -= 1;
            if (CurrentItemIndex < 0) CurrentItemIndex = maxButtonIndex;
            SelectButton(CurrentItemIndex);
            //hoverSound.Play();
        }
    }

    void SelectButton(int index)
    {
        (buttonList[index] as UIInteractable).Select();
    }

    void DeselectButton(int index)
    {
        (buttonList[index] as UIInteractable).Deselect();
    }

    async void SelectButtonEndFrame(int index)
    {
        await ToSignal(GetTree(), "process_frame");
        SelectButton(index);
    }

    public void ResetButtons()
    {
        DeselectButton(CurrentItemIndex);
        CurrentItemIndex = 0;
        SelectButton(CurrentItemIndex);
    }
}
