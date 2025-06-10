
using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class OrbDestruction : Orb
{
    protected override void Collected(Player playerScript)
    {
        playerScript.EnableDestructionMode();
    }
}
