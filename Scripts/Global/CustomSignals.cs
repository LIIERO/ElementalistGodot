using Godot;
using System;
using static GlobalTypes;
using System.Collections.Generic;

public partial class CustomSignals : Node
{
    [Signal]
    public delegate void PlayerAbilityUsedEventHandler(int ability);

    [Signal]
    public delegate void PlayerAbilityListUpdatedEventHandler(int[] abilityArray);
}
