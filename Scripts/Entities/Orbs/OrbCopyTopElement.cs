
using System.Collections.Generic;
using static GlobalTypes;

public partial class OrbCopyTopElement : Orb, IOrb
{
    public void ModifyElementStack(List<ElementState> elementStack)
    {
        if (elementStack.Count > 0)
        {
            elementStack.Add(elementStack[^1]);
        }
    }
}
