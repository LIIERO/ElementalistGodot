using Godot;
using System.Collections.Generic;
using static GlobalTypes;

public abstract partial class Orb : Area2D
{
    [Export] public ColorSet refillColor;
    //Light2D backgroundLight;

    public static GlobalData.PlayerAbilityListEvent PlayerAbilityListModified;

    public override void _Ready()
    {
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
            PlayerAbilityListModified?.Invoke(playerScript.AbilityList);
            QueueFree();
        }
    }
}
