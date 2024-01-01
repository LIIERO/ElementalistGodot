using Godot;
using System.Collections.Generic;
using static GlobalTypes;

public abstract partial class Orb : Area2D
{
    [Export] public ColorSet refillColor;
    //Light2D backgroundLight;

    public override void _Ready()
    {
        //backgroundLight = gameObject.transform.GetChild(0).gameObject.GetComponent<Light2D>();
        //backgroundLight.color = ColorsetToColor[refillColor];
    }

}
