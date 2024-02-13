using Godot;
using System;

public abstract partial class ButtonManager : Node2D
{
    protected GameState gameState;
    private AudioManager audioManager;

    public int CurrentItemIndex { get; private set; }
    protected int startingIndex = 0;
    [Export] protected Node2D[] buttonList; // not only buttons but all interactable stuff from menus like toggles
    //[SerializeField] AudioSource hoverSound;

    int maxButtonIndex;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

        maxButtonIndex = buttonList.Length - 1;
        CurrentItemIndex = startingIndex;
        SelectStartingButtonEndFrame(CurrentItemIndex); // TODO at the end of the frame
    }

    public override void _Process(double delta)
    {
        //if (!buttonList[0].activeInHierarchy) return;

        if (Input.IsActionJustPressed("ui_down"))
        {
            audioManager.buttonSelected.Play();
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex += 1;
            if (CurrentItemIndex > maxButtonIndex) CurrentItemIndex = 0;
            SelectButton(CurrentItemIndex);
            //hoverSound.Play();
        }

        if (Input.IsActionJustPressed("ui_up"))
        {
            audioManager.buttonSelected.Play();
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

    async void SelectStartingButtonEndFrame(int index)
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
