
using System.Collections.Generic;
using static GlobalTypes;

public partial class OrbAddElement : Orb, IOrb
{
    public void ModifyElementStack(List<ElementState> elementStack)
    {
        elementStack.Add(ColorsetToElement[refillColor]);
    }
}
