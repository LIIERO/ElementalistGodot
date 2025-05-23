using Godot;
using System.Collections.Generic;
using GlobalTypes;

public partial class ElementalShell : PermanentCollectable
{

    protected override void OnCollected()
    {
        gameState.IsAbilitySalvagingUnlocked = true;
    }

    protected override bool IsCollected()
    {
        return gameState.IsAbilitySalvagingUnlocked;
    }
}
