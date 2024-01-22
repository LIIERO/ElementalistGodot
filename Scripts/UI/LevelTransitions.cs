using Godot;
using System;
using GlobalTypes;

public partial class LevelTransitions : CanvasLayer
{
	private AnimationPlayer animationPlayer;
	private GameState gameState; // singleton

    [Export] private Label levelTextTopLabel;
    [Export] private Label levelTextBottomLabel;
    private LevelData transitionLevel;
    private ScreenTransition currentTransition;

	public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        HideTransition();
    }

    private void HideTransition()
    {
        Hide();
        levelTextTopLabel.Hide();
        levelTextBottomLabel.Hide();
    }

    private void EndLevelTransition()
	{
		animationPlayer.Play("LevelEnter");
        gameState.IsLevelTransitionPlaying = false;
    }

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
        }

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
            }
        }

		if (animationName == "LevelEnter") // End of transition
		{
            HideTransition();
        }
	}

    private async void EndLevelTransitionAfterSeconds(float t)
    {
        await ToSignal(GetTree().CreateTimer(t), "timeout");
        EndLevelTransition();
    }
}
