
using Godot;
using System.Collections.Generic;
using static GlobalTypes;

public partial class OrbAddElement : Orb
{
    protected override void ModifyElementStack(List<ElementState> elementStack)
    {
        elementStack.Add(ColorsetToElement[refillColor]);
    }
}
