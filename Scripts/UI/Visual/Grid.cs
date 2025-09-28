using Godot;
using System;

public partial class Grid : ParallaxBackground
{
    public override void _Ready()
	{
        if (GetNode<SettingsManager>("/root/SettingsManager").GridEnabled)
		{
			Show();
		}
		else
		{
			QueueFree();
		}
    }
}
