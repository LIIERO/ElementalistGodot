using Godot;
using System;

public partial class SaveSlot : Node2D
{
	[Export] private string slotID = "0";

	private GameState gameState;

	private Label slotName;
	private Label worldName;
	private Label levelName;
	private Label time;
	private Label deaths;
	private Label restarts;
	private Label undos;

	public override void _Ready()
	{
		gameState = GetNode<GameState>("/root/GameState");

		slotName = GetNode<Label>("Sprite2D/SlotName");
		worldName = GetNode<Label>("Sprite2D/WorldName");
		//levelName = GetNode<Label>("Sprite2D/LevelName");
		time = GetNode<Label>("Sprite2D/Time");
		deaths = GetNode<Label>("Sprite2D/Deaths");
		restarts = GetNode<Label>("Sprite2D/Restarts");
		undos = GetNode<Label>("Sprite2D/Undos");

		PlayerData savefileData = gameState.GetSaveFileData(slotID);

		slotName.Text = $"SLOT {slotID}";

		if (savefileData != null)
		{
			worldName.Text = $"Level {savefileData.CurrentWorld}-{savefileData.CurrentLevel}";
			//levelName.Text = savefileData.CurrentLevel;
			time.Text = GameUtils.FormatTime(savefileData.InGameTime);
			deaths.Text = savefileData.NoDeaths.ToString();
			restarts.Text = savefileData.NoRestarts.ToString();
			undos.Text = savefileData.NoUndos.ToString();
		}
	}

	
	public override void _Process(double delta)
	{
	}
}
