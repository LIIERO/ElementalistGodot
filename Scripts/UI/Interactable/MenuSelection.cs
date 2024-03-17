using Godot;
using System;

public partial class MenuSelection : UIInteractable
{

    public int CurrentValueIndex { get; private set; } = 0;
    public bool Enabled { get; private set; } = true;

    [Export] private string[] values;
    private int lastIndex;

    private Sprite2D selectionPanel;
    private Label valueDisplay;
    private Node2D leftArrow;
    private Node2D rightArrow;

    private AudioManager audioManager; // singleton

    public override void _Ready()
	{
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

        lastIndex = values.Length - 1;
        selectionPanel = GetNode<Sprite2D>("Selection");
        valueDisplay = GetNode<Label>("Selection/Text");
        leftArrow = GetNode<Node2D>("Selection/ArrowIndicatorThick");
        rightArrow = GetNode<Node2D>("Selection/ArrowIndicatorThick2");

        base._Ready();
    }

	public override void Select()
	{
        if (Enabled) UpdateArrows();

        base.Select();
    }

	public override void Deselect()
	{
        leftArrow.Hide();
        rightArrow.Hide();

        base.Deselect();
    }


    public void SetCurrentValueIndex(int index)
    {
        if (index > lastIndex) return;

        CurrentValueIndex = index;
        RefreshDisplay();
    }

    public bool MoveRight()
    {
        if (!Enabled || CurrentValueIndex >= lastIndex) return false;
        CurrentValueIndex++;
        RefreshDisplay();
        UpdateArrows();
        audioManager.buttonSelected.Play();
        return true;
    }

    public bool MoveLeft()
    {
        if (!Enabled || CurrentValueIndex <= 0) return false;
        CurrentValueIndex--;
        RefreshDisplay();
        UpdateArrows();
        audioManager.buttonSelected.Play();
        return true;
    }

    public string GetCurrentValue()
    {
        return values[CurrentValueIndex];
    }

    public void Disable()
    {
        selectionPanel.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Enabled = false;
    }

    public void Enable()
    {
        selectionPanel.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Enabled = true;
    }

    private void RefreshDisplay()
    {
        valueDisplay.Text = values[CurrentValueIndex];
    }

    private void UpdateArrows()
    {
        if (CurrentValueIndex == lastIndex) rightArrow.Hide();
        else rightArrow.Show();
        if (CurrentValueIndex == 0) leftArrow.Hide();
        else leftArrow.Show();
    }
}
