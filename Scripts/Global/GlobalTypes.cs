
using System.Collections.Generic;


public static class GlobalTypes
{
    public enum ColorSet { red, blue, white, brown, green }; // Global color set
    public enum ElementState { normal, water, fire, air, earth };
    public enum InteractableType { teleport, checkpoint, sign, entrance }; // checkpoint is also a teleport that brings you back
    public enum Direction { left, right, up, down };

    /*public static readonly Dictionary<ColorSet, Color> ColorsetToColor = new() { 
        { ColorSet.red, new Color(1f, 0.2f, 0.2f) },
        { ColorSet.blue, new Color(0.2f, 0.2f, 1f) },
        { ColorSet.white, new Color(1f, 1f, 1f) },
        { ColorSet.brown, new Color(0.588f, 0.353f, 0.118f) },
        { ColorSet.green, new Color(0.2f, 1f, 0.2f) }
    };*/

    public static readonly Dictionary<ColorSet, ElementState> ColorsetToElement = new() {
        { ColorSet.red, ElementState.fire },
        { ColorSet.blue, ElementState.water },
        { ColorSet.white, ElementState.air },
        { ColorSet.brown, ElementState.earth }
    };

    public enum ScreenTransition { empty, levelEntry, levelCompleted };
}
