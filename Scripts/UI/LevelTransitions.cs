using Godot;
using System;
using GlobalTypes;

public partial class LevelTransitions : CanvasLayer
{
	private AnimationPlayer animationPlayer;

    // singletons
    private GameState gameState;
    private AudioManager audioManager;

    [Export] private Label levelTextTopLabel;
    [Export] private Label levelTextBottomLabel;

    private LevelData transitionLevel;
    private WorldData transitionWorld;

    private ScreenTransition currentTransition;

	public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        HideTransition();
    }
    
    private void HideTransition()
    {
        Hide();
        levelTextTopLabel.Hide();
        levelTextBottomLabel.Hide();
        gameState.IsLevelTransitionPlaying = false;
    }

    private void EndLevelTransition()
	{
		animationPlayer.Play("LevelEnter");
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
        levelTextTopLabel.Text = "Level " + levelToTransitionTo.ID;
        levelTextBottomLabel.Text = levelToTransitionTo.Name;

        animationPlayer.Play("LevelExit");
    }

    public void StartLevelReloadTransition()
    {
        currentTransition = ScreenTransition.restart;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit");
    }

    public void StartHubTransition(bool levelCompleted)
    {
        currentTransition = ScreenTransition.hubEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();

        if (levelCompleted)
        {
            currentTransition = ScreenTransition.hubEntryCompleted;
            levelTextBottomLabel.Show();
            levelTextBottomLabel.Text = "Level complete";
            audioManager.levelCompleted.Play();
        }

        animationPlayer.Play("LevelExit");
    }

    public void StartWorldTransition(WorldData worldToTransitionTo)
    {
        currentTransition = ScreenTransition.worldEntry;
        gameState.IsLevelTransitionPlaying = true;
        transitionWorld = worldToTransitionTo;
        Show();

        levelTextTopLabel.Show();
        levelTextBottomLabel.Show();
        levelTextTopLabel.Text = "World " + worldToTransitionTo.ID;
        levelTextBottomLabel.Text = worldToTransitionTo.Name;

        animationPlayer.Play("LevelExit");
    }

    // Menu transitions

    public void StartOptionsTransition()
    {
        currentTransition = ScreenTransition.optionsEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit");
    }

    public void StartMenuTransition()
    {
        currentTransition = ScreenTransition.menuEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit");
    }

    public void StartGameTransition()
    {
        currentTransition = ScreenTransition.gameEntry;
        gameState.IsLevelTransitionPlaying = true;
        Show();
        animationPlayer.Play("LevelExit");
    }


    void _onAnimationPlayerAnimationFinished(string animationName)
	{
        if (animationName == "LevelExit") // End of first part of transition
        {
            switch (currentTransition)
            {
                case ScreenTransition.restart:
                    gameState.RestartCurrentLevel();
                    EndLevelTransition(); break;

                case ScreenTransition.hubEntry:
                    gameState.LoadHubLevel();
                    EndLevelTransition(); break;

                case ScreenTransition.hubEntryCompleted:
                    gameState.LoadHubLevel();
                    EndLevelTransitionAfterSeconds(0.5f); break;

                case ScreenTransition.levelEntry:
                    gameState.LoadLevel(transitionLevel.ID);
                    EndLevelTransitionAfterSeconds(1.5f); break;

                case ScreenTransition.worldEntry:
                    audioManager.StopMusicWithFade();
                    gameState.LoadWorld(transitionWorld.ID);
                    EndLevelTransitionAfterSeconds(1.5f); break;

                case ScreenTransition.optionsEntry:
                    gameState.LoadOptions();
                    EndLevelTransition(); break;

                case ScreenTransition.menuEntry:
                    audioManager.StopMusicWithFade();
                    gameState.LoadMenu();
                    EndLevelTransition(); break;

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

    private async void EndLevelTransitionAfterSeconds(float t)
    {
        await ToSignal(GetTree().CreateTimer(t), "timeout");
        EndLevelTransition();
    }
}
