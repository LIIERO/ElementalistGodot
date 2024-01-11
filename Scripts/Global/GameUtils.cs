using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;

public static class GameUtils

{
    public static readonly int gameUnitSize = 16;

    public static bool PlayingCutscene { get; set; } = false; // TODO MOVE THIS AWAY FROM HERE

    public static readonly Dictionary<ColorSet, Color> ColorsetToColor = new() {
        { ColorSet.red, new Color(1f, 0.2f, 0.2f) },
        { ColorSet.blue, new Color(0.2f, 0.2f, 1f) },
        { ColorSet.white, new Color(1f, 1f, 1f) },
        { ColorSet.brown, new Color(0.588f, 0.353f, 0.118f) },
        { ColorSet.green, new Color(0.2f, 1f, 0.2f) }
    };

    public static readonly Dictionary<ColorSet, ElementState> ColorsetToElement = new() {
        { ColorSet.red, ElementState.fire },
        { ColorSet.blue, ElementState.water },
        { ColorSet.white, ElementState.air },
        { ColorSet.brown, ElementState.earth }
    };

    public static readonly Dictionary<ElementState, ColorSet> ElementToColorset = new() {
        { ElementState.fire, ColorSet.red },
        { ElementState.water , ColorSet.blue },
        { ElementState.air , ColorSet.white },
        { ElementState.earth , ColorSet.brown }
    };

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
