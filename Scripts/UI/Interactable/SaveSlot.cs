using Godot;
using System;

public partial class SaveSlot : Node2D
{
	[Export] public string SlotID { get; private set; } = "0";

	private GameState gameState;

	private Node2D arrowIndicator;

	private Color selectedModulate = new(1f, 1f, 1f);
	private Color deselectedModulate = new(0.6f, 0.6f, 0.6f);

    private Label slotName;
	private Label worldName;
	private Label sunFragments;
    private Label redFragments;
    private Label time;
	private Label deaths;
	private Label restarts;
	private Label undos;
    private Label abilityUses;

    private Vector2 basePosition;
    private Vector2 offsetPosition;

    public override void _Ready()
	{
		gameState = GetNode<GameState>("/root/GameState");

        basePosition = Position;
        offsetPosition = basePosition + new Vector2(0.0f, GameUtils.gameUnitSize);

		Modulate = deselectedModulate;

		arrowIndicator = GetNode<Node2D>("ArrowIndicatorThick");
		arrowIndicator.Hide();
        slotName = GetNode<Label>("Sprite2D/SlotName");
		worldName = GetNode<Label>("Sprite2D/WorldName");
		sunFragments = GetNode<Label>("Sprite2D/SunFragments");
        redFragments = GetNode<Label>("Sprite2D/RedFragments");
        time = GetNode<Label>("Sprite2D/Time");
		deaths = GetNode<Label>("Sprite2D/Deaths");
		restarts = GetNode<Label>("Sprite2D/Restarts");
		undos = GetNode<Label>("Sprite2D/Undos");
        abilityUses = GetNode<Label>("Sprite2D/AbilityUses");

        PlayerData savefileData = gameState.GetSaveFileData(SlotID);

		slotName.Text = $"{gameState.UITextData["slot"]} {SlotID}";

		if (savefileData != null)
		{
			worldName.Text = $"{gameState.UITextData["level"]} {savefileData.CurrentWorld}-{savefileData.CurrentLevel}";
			sunFragments.Text = savefileData.NoSunFragments.ToString();
			redFragments.Text = savefileData.NoRedFragments.ToString();
			time.Text = GameUtils.FormatTime(savefileData.InGameTime);
			deaths.Text = savefileData.NoDeaths.ToString();
			restarts.Text = savefileData.NoRestarts.ToString();
			undos.Text = savefileData.NoUndos.ToString();
			abilityUses.Text = savefileData.NoAbilityUses.ToString();
		}
	}

	public void Select()
	{
        Position = offsetPosition;
		arrowIndicator.Show();
		Modulate = selectedModulate;
    }

    public void Deselect()
    {
		arrowIndicator.Hide();
        Position = basePosition;
		Modulate = deselectedModulate;
    }
}
