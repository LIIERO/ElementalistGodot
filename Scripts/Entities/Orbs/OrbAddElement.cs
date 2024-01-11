
using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class OrbAddElement : Orb
{
    protected override void ModifyElementStack(List<ElementState> elementStack)
    {
        elementStack.Add(GameUtils.ColorsetToElement[refillColor]);
    }
}
