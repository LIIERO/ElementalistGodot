using Godot;
using System;
using System.Collections.Generic;
using GlobalTypes;

public static class GameUtils

{
    public static readonly int gameUnitSize = 16;
    public static readonly string wordToMake = "ZOE";

    public static readonly Dictionary<ColorSet, Color> ColorsetToColor = new() {
        { ColorSet.red, new Color(1f, 0.2f, 0.2f) },
        { ColorSet.blue, new Color(0.2f, 0.2f, 1f) },
        { ColorSet.white, new Color(1f, 1f, 1f) },
        { ColorSet.brown, new Color(0.588f, 0.353f, 0.118f) },
        { ColorSet.green, new Color(0.2f, 1f, 0.2f) },
        { ColorSet.yellow, new Color(1f, 1f, 0.2f) },
        { ColorSet.purple, new Color(1f, 0.2f, 1f) },
        { ColorSet.pink, new Color(1f, 0.0f, 0.5f) },
        { ColorSet.black, new Color(0.2f, 0.2f, 0.2f) },
        { ColorSet.cyan, new Color(0.2f, 1f, 1f) }
    };

    public static readonly Dictionary<ColorSet, ElementState> ColorsetToElement = new() {
        { ColorSet.red, ElementState.fire },
        { ColorSet.blue, ElementState.water },
        { ColorSet.white, ElementState.air },
        { ColorSet.brown, ElementState.earth },
        { ColorSet.pink, ElementState.love },
    };

    public static readonly Dictionary<ElementState, ColorSet> ElementToColorset = new() {
        { ElementState.fire, ColorSet.red },
        { ElementState.water , ColorSet.blue },
        { ElementState.air , ColorSet.white },
        { ElementState.earth , ColorSet.brown },
        { ElementState.love , ColorSet.pink },
        { ElementState.earth_remix , ColorSet.brown },
        { ElementState.normal , ColorSet.green }
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

    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        float num4 = current - target;
        float num5 = target;
        float num6 = maxSpeed * smoothTime;
        num4 = Mathf.Clamp(num4, -num6, num6);
        target = current - num4;
        float num7 = (currentVelocity + num * num4) * deltaTime;
        currentVelocity = (currentVelocity - num * num7) * num3;
        float num8 = target + (num4 + num7) * num3;
        if (num5 - current > 0f == num8 > num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }

    public static float LinearToDecibel(float linear)
    {
        float dB;
        if (linear > 0.001f)
            dB = 20.0f * (float)Math.Log10(linear);
        else
            dB = -144.0f;
        return dB;
    }

    public static bool LevelIDEndsWithLetter(string id, string letter)
    {
        return id.Length > 1 && id.EndsWith(letter);
    }

    public static void ListRemoveLastElement<T>(List<T> list)
    {
        list.RemoveAt(list.Count - 1);
    }

    public static string FormatTime(double time)
    {
        int seconds = (int)time;
        int miliseconds = (int)(time%1 * 1000);

        TimeSpan span = new(0, 0, seconds);
        return string.Format("{0:0}:{1:00}:{2:00}.{3:000}", span.Hours, span.Minutes, span.Seconds, miliseconds);
    }


    public class DataFileDoesntExistException : Exception
    {
        public DataFileDoesntExistException(string message) : base(message) { }
    }
}
