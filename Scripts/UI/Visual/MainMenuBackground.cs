using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class MainMenuBackground : Node2D
{
	[Export] public PackedScene[] backgrounds;
    [Export] public string[] worldIDs;

    private GameState gameState;

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");

        PlayerData data = gameState.GetSaveFileData(gameState.CurrentSaveFileID);
        if (data == null)
        {
            GD.Print("Path not found (save file)");
            AddChild(backgrounds[0].Instantiate() as ParallaxBackground);
            return;
        }

        for (int i = 0; i < worldIDs.Length; i++)
        {
            if (worldIDs[i] == data.CurrentWorld)
            {
                ParallaxBackground bg = backgrounds[i].Instantiate() as ParallaxBackground;
                AddChild(bg);
                return;
            }
        }
        AddChild(backgrounds[0].Instantiate() as ParallaxBackground);
    }
}
