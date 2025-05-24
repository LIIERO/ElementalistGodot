using Godot;
using System;

public partial class SaveSlot : Node2D
{
	[Export] public string SlotID { get; private set; } = "0";

	private GameState gameState;

	private Node2D arrowIndicator;

    private Label slotName;
	private Label worldName;
	private Label sunFragments;
    private Label redFragments;
    private Label time;
	private Label deaths;
	private Label restarts;
	private Label undos;
    private Label abilityUses;

    private Node2D permanentUnlocks;
    private Sprite2D elementalShell;
    private AnimatedSprite2D[] letters = new AnimatedSprite2D[GameUtils.wordToMake.Length];

    private Vector2 basePosition;
    private Vector2 offsetPosition;

    public override void _Ready()
	{
		gameState = GetNode<GameState>("/root/GameState");

        basePosition = Position;
        offsetPosition = basePosition + new Vector2(0.0f, GameUtils.gameUnitSize);

        SetModulateToDeselected();

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

        permanentUnlocks = GetNode<Node2D>("Sprite2D/PermanentUnlocks");
        permanentUnlocks.Hide();
        elementalShell = GetNode<Sprite2D>("Sprite2D/PermanentUnlocks/ElementalShell");
        for (int i = 0; i < letters.Length; i++)
            letters[i] = GetNode<AnimatedSprite2D>("Sprite2D/PermanentUnlocks/" + GameUtils.wordToMake[i]);
        
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

            SetCollectableDisplay(savefileData);
		}
    }

	public void Select()
	{
        Position = offsetPosition;
		arrowIndicator.Show();
		SetModulateToSelected();
    }

    public void Deselect()
    {
		arrowIndicator.Hide();
        Position = basePosition;
		SetModulateToDeselected();
    }

    public void SetModulateToSelected()
    {
        Modulate = new Color(1f, 1f, 1f, Modulate.A);
    }

    public void SetModulateToDeselected()
    {
        Modulate = new Color(0.6f, 0.6f, 0.6f, Modulate.A);
    }

    public void SetOpacityToHalf()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.5f);
    }

    public void SetOpacityToNormal()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
    }

    private void SetCollectableDisplay(PlayerData saveData) // Code copied from CollectableDisplay.cs, too lazy to make another class to reuse it
    {
        if (!saveData.IsAbilitySalvagingUnlocked && saveData.UnlockedLetters.Count == 0)
        {
            return;
        }

        permanentUnlocks.Show();

        if (saveData.IsAbilitySalvagingUnlocked)
            elementalShell.Show();
        else
            elementalShell.Hide();

        if (saveData.UnlockedLetters == null) saveData.UnlockedLetters = new(); // Incompatible save file fix

        for (int i = 0; i < GameUtils.wordToMake.Length; i++)
        {
            string l = GameUtils.wordToMake[i].ToString();
            if (saveData.UnlockedLetters.Contains(l))
            {
                letters[i].Play(l);
            }
            else
            {
                letters[i].Play("_");
            }
        }
    }
}
