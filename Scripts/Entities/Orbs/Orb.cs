using Godot;
using System.Collections.Generic;
using static GlobalTypes;

public abstract partial class Orb : Area2D
{
	[Export] public ColorSet refillColor;
	//Light2D backgroundLight;

    // Signals
    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        //backgroundLight = gameObject.transform.GetChild(0).gameObject.GetComponent<Light2D>();
        //backgroundLight.color = ColorsetToColor[refillColor];
    }

	protected virtual void ModifyElementStack(List<ElementState> elementStack) {}

	void _on_body_entered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			// Get the script of player node and modify it's ability list
			Player playerScript = body as Player;
			ModifyElementStack(playerScript.AbilityList);
            customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityListUpdated, GlobalUtils.ElementListToIntArray(playerScript.AbilityList));
            //PlayerAbilityListModified?.Invoke(playerScript.AbilityList);
			QueueFree();
		}
	}

}
