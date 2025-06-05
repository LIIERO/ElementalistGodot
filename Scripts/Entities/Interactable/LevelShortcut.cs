using Godot;
using System;
using System.Security.AccessControl;
using static System.Net.Mime.MediaTypeNames;

public partial class LevelShortcut : Interactable
{
    // Singletons
    private GameState gameState;
    private CustomSignals customSignals;
    private LevelTransitions levelTransitions;

    private Label teleportText;

    [Export] private string text = "level_shortcut_info";
    [Export] private LevelData[] orderedLevels;
    private LevelData levelToTeleportTo = null;

    public static bool setPlayerLevelEnterPosition = false;

    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;

        teleportText = GetNode<Label>("MovedByAnimation/Text/Label");

        base._Ready();

        for (int i = 0; i < orderedLevels.Length; i++)
        {
            LevelData level = orderedLevels[i];
            if (gameState.HasLevelBeenCompleted(level.ID)) // TODO: last entered level (not completed)
            {
                levelToTeleportTo = level;
            }
        }

        teleportText.Text = levelToTeleportTo == null ? "" : levelToTeleportTo.ID[0].ToString();
    }


    protected override void Interact()
    {
        base.Interact();

        if (levelToTeleportTo == null)
        {
            customSignals.EmitSignal(CustomSignals.SignalName.StartDialog, text);
            return;
        }

        gameState.SalvagedAbilities = new();

        levelTransitions.StartLevelTransition(levelToTeleportTo);
        if (levelToTeleportTo.IsSpecial) gameState.IsCurrentLevelSpecial = true;
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        customSignals.EmitSignal(CustomSignals.SignalName.EndDialog);
    }
}
