using Godot;
using System;

public partial class LocalGameManager : Node
{
    // Delete this class I think
    void _OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
