
using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class OrbAddElement : Orb
{
    protected override void ModifyElementStack(List<ElementState> elementStack)
    {
        if (isRemixed && refillColor == ColorSet.brown)
        {
            elementStack.Add(ElementState.earth_remix);
            return;
        }

        elementStack.Add(GameUtils.ColorsetToElement[refillColor]);
    }
}
