
using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class OrbReverse : Orb
{
    protected override void ModifyElementStack(List<ElementState> elementStack)
    {
        elementStack.Reverse();
    }
}
