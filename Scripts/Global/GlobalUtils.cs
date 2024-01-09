using Godot;
using System;
using System.Collections.Generic;
using static GlobalTypes;

public static class GlobalUtils

{
    public static readonly int gameUnitSize = 16;

    public static bool PlayingCutscene { get; set; } = false; // TODO MOVE THIS AWAY FROM HERE

    public static int[] ElementListToIntArray(List<ElementState> elementList)
    {
        int[] elementArray = new int[elementList.Count];
        for (int i = 0; i < elementList.Count; i++)
        {
            elementArray[i] = (int)elementList[i];
        }
        return elementArray;
    }

    public static string GetKeyName(string actionName) // Fix this
    {
        //OS.get_scancode_string(InputMap.get_action_list("interact")[0].physical_scancode)
        //var keycode = DisplayServer.KeyboardGetKeycodeFromPhysical(inputEventKey.PhysicalKeycode);
        //GD.Print(OS.GetKeycodeString(keycode));
        //return OS.GetKeycodeString();
        return InputMap.ActionGetEvents(actionName)[0].AsText();
    }
}
