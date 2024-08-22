
using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class OrbRemoveTopElement : Orb
{
    protected override void ModifyElementStack(List<ElementState> elementStack)
    {
        if (elementStack.Count > 0)
        {
            GameUtils.ListRemoveLastElement(elementStack);
        }
    }
}
