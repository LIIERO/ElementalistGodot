using Godot;
using System;

public abstract partial class ButtonManager : Control
{
    protected GameState gameState;
    private AudioManager audioManager;

    public int CurrentItemIndex { get; private set; }
    protected int startingIndex = 0;
    [Export] protected UIInteractable[] buttonList; // not only buttons but all interactable stuff from menus like toggles
    //[SerializeField] AudioSource hoverSound;

    private int maxButtonIndex;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

        maxButtonIndex = buttonList.Length - 1;
        CurrentItemIndex = startingIndex;
        SelectStartingButtonEndFrame(CurrentItemIndex);
    }

    public override void _Process(double delta)
    {
        //if (!buttonList[0].activeInHierarchy) return;

        if (InputManager.UIDownPressed())
        {
            audioManager.buttonSelected.Play();
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex += 1;
            if (CurrentItemIndex > maxButtonIndex) CurrentItemIndex = 0;
            SelectButton(CurrentItemIndex);
        }

        if (InputManager.UIUpPressed())
        {
            audioManager.buttonSelected.Play();
            DeselectButton(CurrentItemIndex);
            CurrentItemIndex -= 1;
            if (CurrentItemIndex < 0) CurrentItemIndex = maxButtonIndex;
            SelectButton(CurrentItemIndex);
        }
    }

    protected void SelectButton(int index)
    {
        buttonList[index].Select();
    }

    protected void DeselectButton(int index)
    {
        buttonList[index].Deselect();
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
