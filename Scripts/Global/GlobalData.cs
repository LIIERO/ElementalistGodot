using Godot;
using System;
using System.Collections.Generic;
using static GlobalTypes;

public static class GlobalData
{
    public delegate void PlayerAbilityListEvent(List<ElementState> abilityList);

    public delegate void PlayerAbilityEvent(ElementState ability);

    public static readonly int gameUnitSize = 16;

    public static bool PlayingCutscene { get; set; } = false;

}
