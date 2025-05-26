using Godot;
using System;
using GlobalTypes;

public partial class LevelTransitions : CanvasLayer
{
	private AnimationPlayer animationPlayer;

    // singletons
    private GameState gameState;
    private AudioManager audioManager;
    private CustomSignals customSignals;

    [Export] private Label levelTextTopLabel;
    [Export] private Label levelTextBottomLabel;

    private LevelData transitionLevel;
    private WorldData transitionWorld;

    private ScreenTransition currentTransition;

    private const float shortSpeedMul = 1.25f;
    // Short transitions: reload, options, credits, menu

	public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        HideTransition();
    }
    
    private void HideTransition()
    {
        Hide();
        levelTextTopLabel.Hide();
        levelTextBottomLabel.Hide();
        gameState.IsLevelTransitionPlaying = false;
        customSignals.EmitSignal(CustomSignals.SignalName.LevelTransitioned);
    }

    private void EndLevelTransition(float speedMul = 1.0f)
	{
		animationPlayer.Play("LevelEnter", customSpeed:speedMul);
        audioManager.fadeOut.Play();
    }

    // Gameplay transitions

    public void StartLevelTransition(LevelData levelToTransitionTo)
    {
        currentTransition = ScreenTransition.levelEntry;
        gameState.IsLevelTransitionPlaying = true;
		transitionLevel = levelToTransitionTo;
        Show();

        levelTextTopLabel.Show();
        levelTextBottomLabel.Show();
        levelTextTopLabel.Text = string.Format($"{gameState.UITextData["level_transition"]} {gameState.CurrentWorld}-{levelToTransitionTo.ID}");
        levelTextBottomLabel.Text = gameState.GetLevelName(levelToTransitionTo.Name);

        animationPlayer.Play("LevelExit");
        audioManager.fadeIn.Play();
    }

    public void StartLevelReloadTransition()
    {
        currentTransition = ScreenTransition.restart;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit", customSpeed:shortSpeedMul);
        audioManager.fadeIn.Play();
    }

    public void StartHubTransition(bool levelCompleted)
    {
        currentTransition = ScreenTransition.hubEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();

        if (levelCompleted)
        {
            currentTransition = ScreenTransition.hubEntryCompleted;

            if (gameState.SalvagedAbilities.Count > 0) // Salvaged abilities
            {
                levelTextTopLabel.Show();
                levelTextTopLabel.Text = gameState.UITextData["abilities_salvaged"];
            }

            levelTextBottomLabel.Show();
            levelTextBottomLabel.Text = gameState.IsCurrentLevelSpecial ? gameState.UITextData["level_complete_special"] : gameState.UITextData["level_complete"];
            audioManager.levelCompleted.Play();
        }

        animationPlayer.Play("LevelExit");
        audioManager.fadeIn.Play();
    }

    public void StartWorldTransition(WorldData worldToTransitionTo)
    {
        currentTransition = ScreenTransition.worldEntry;
        gameState.IsLevelTransitionPlaying = true;
        transitionWorld = worldToTransitionTo;
        Show();

        levelTextTopLabel.Show();
        levelTextBottomLabel.Show();
        levelTextTopLabel.Text = string.Format($"{gameState.UITextData["world_transition"]} {worldToTransitionTo.ID}");
        levelTextBottomLabel.Text = gameState.GetLevelName(worldToTransitionTo.Name);

        animationPlayer.Play("LevelExit");
        audioManager.fadeIn.Play();
    }

    // Menu transitions

    public void StartOptionsTransition()
    {
        currentTransition = ScreenTransition.optionsEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit", customSpeed: shortSpeedMul);
        audioManager.fadeIn.Play();
    }

    public void StartCreditsTransition()
    {
        currentTransition = ScreenTransition.creditsEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit", customSpeed: shortSpeedMul);
        audioManager.fadeIn.Play();
    }

    public void StartMenuTransition()
    {
        currentTransition = ScreenTransition.menuEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit", customSpeed: shortSpeedMul);
        audioManager.fadeIn.Play();
    }

    public void StartGameTransition()
    {
        currentTransition = ScreenTransition.gameEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit");
        audioManager.fadeIn.Play();
    }


    void _onAnimationPlayerAnimationFinished(string animationName)
	{
        if (animationName == "LevelExit") // End of first part of transition
        {
            switch (currentTransition)
            {
                case ScreenTransition.restart:
                    gameState.RestartCurrentLevel();
                    EndLevelTransition(shortSpeedMul); break;

                case ScreenTransition.hubEntry:
                    gameState.LoadHubLevel();
                    EndLevelTransition(); break;

                case ScreenTransition.hubEntryCompleted:
                    gameState.LoadHubLevel();
                    EndLevelTransitionAfterSeconds(0.5f); break;

                case ScreenTransition.levelEntry:
                    gameState.LoadLevel(transitionLevel.ID, gameState.GetLevelName(transitionLevel.Name));
                    EndLevelTransitionAfterSeconds(1.5f); break;

                case ScreenTransition.worldEntry:
                    audioManager.StopMusicWithFade();
                    gameState.LoadWorld(transitionWorld.ID);
                    EndLevelTransitionAfterSeconds(1.5f); break;

                case ScreenTransition.optionsEntry:
                    gameState.LoadOptions();
                    EndLevelTransition(shortSpeedMul); break;

                case ScreenTransition.creditsEntry:
                    gameState.LoadCredits();
                    EndLevelTransition(shortSpeedMul); break;

                case ScreenTransition.menuEntry:
                    audioManager.StopMusicWithFade();
                    gameState.LoadMenu();
                    EndLevelTransition(shortSpeedMul); break;

                case ScreenTransition.gameEntry:
                    gameState.LoadGame();
                    EndLevelTransition(); break;
            }
        }

		if (animationName == "LevelEnter") // End of transition
		{
            HideTransition();

            if (currentTransition == ScreenTransition.gameEntry || currentTransition == ScreenTransition.worldEntry)
            {
                audioManager.PlayWorldMusic(gameState.CurrentWorld); // Start music upon entering the game or a new world
            }
        }
	}

    private async void EndLevelTransitionAfterSeconds(float t, float speedMul = 1.0f)
    {
        await ToSignal(GetTree().CreateTimer(t), "timeout");
        EndLevelTransition(speedMul);
    }
}
