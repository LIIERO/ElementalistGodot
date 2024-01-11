using Godot;
using System.Collections.Generic;
using GlobalTypes;

public abstract partial class Orb : Area2D
{
	[Export] public ColorSet refillColor;
	[Export] Light2D backgroundLight;
    [Export] CpuParticles2D particles;

    // Signals
    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        backgroundLight.Color = GameUtils.ColorsetToColor[refillColor];
        particles.Color = GameUtils.ColorsetToColor[refillColor];
    }

	protected virtual void ModifyElementStack(List<ElementState> elementStack) {}

	void _OnBodyEntered(Node2D body)
	{
        if (body is not Player) return;

        // Get the script of player node and modify its ability list
        Player playerScript = body as Player;
		ModifyElementStack(playerScript.AbilityList);
        customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityListUpdated, GameUtils.ElementListToIntArray(playerScript.AbilityList));
		QueueFree();
	}

}
