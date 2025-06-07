using Godot;
using System;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;

public partial class Watchtower : Interactable
{
    private GameState gameState; // Singleton
    //private bool unlockNextFrame = false;

    private Light2D backgroundLight;

    public override void _Ready()
	{
		base._Ready();

        gameState = GetNode<GameState>("/root/GameState");
        backgroundLight = GetNode<PointLight2D>("MovedByAnimation/PointLight2D");

        bool lightParticlesActive = GetNode<SettingsManager>("/root/SettingsManager").LightParticlesActive;
        if (!lightParticlesActive)
            backgroundLight.QueueFree();
    }

    public override void _Process(double delta)
    {

        /*if (unlockNextFrame)
        {
            UnlockWatchtowerEndFrame();
            return;
        }  */

        if (!gameState.WatchtowerActive) return;

        if (InputManager.JumpPressed() || InputManager.AbilityPressed() || InputManager.UndoPressed())
        {
            gameState.WatchtowerActive = false;
            //unlockNextFrame = true;
        }
    }

    protected override void Interact()
	{
        /*if (unlockNextFrame) return;

        unlockNextFrame = true;*/

		base.Interact();

        if (gameState.WatchtowerActive) return;

        gameState.WatchtowerActive = true;
    }

    protected override void PlayerExited()
    {
        base.PlayerExited();

        gameState.WatchtowerActive = false;
    }

    /*async void UnlockWatchtowerEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");
        unlockNextFrame = false;
    }*/
}
