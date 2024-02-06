using Godot;
using System.Collections.Generic;
using GlobalTypes;

public abstract partial class Orb : Area2D
{
	[Export] public ColorSet refillColor;
	private Light2D backgroundLight;
    private CpuParticles2D particles;

    // Singletons
    private AudioManager audioManager;
    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

        backgroundLight = GetNode<PointLight2D>("MovedByAnimation/PointLight2D");
        particles = GetNode<CpuParticles2D>("MovedByAnimation/GPUParticles2D");

        backgroundLight.Color = GameUtils.ColorsetToColor[refillColor];
        particles.Color = GameUtils.ColorsetToColor[refillColor];
    }

	protected virtual void ModifyElementStack(List<ElementState> elementStack) {}

	void _OnBodyEntered(Node2D body)
	{
        if (body is not Player) return;

        // Get the script of player node and modify its ability list
        Player playerScript = body as Player;
        audioManager.orbCollectSound.Play();
		ModifyElementStack(playerScript.AbilityList);
        customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityListUpdated, GameUtils.ElementListToIntArray(playerScript.AbilityList));
		QueueFree();
	}

}
