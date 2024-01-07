using Godot;
using System;
using System.Collections.Generic;
using static GlobalTypes;

public static class GlobalUtils

{
    public static readonly int gameUnitSize = 16;

    public static bool PlayingCutscene { get; set; } = false; // TODO MOVE THIS AWATY FROM HERE

    public static int[] ElementListToIntArray(List<ElementState> elementList)
    {
        int[] elementArray = new int[elementList.Count];
        for (int i = 0; i < elementList.Count; i++)
        {
            elementArray[i] = (int)elementList[i];
        }
        return elementArray;
    }
}
