using Godot;
using System;
using static GlobalTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

public partial class ElementOverlay : Node2D
{
	[Export] private PackedScene elementSymbol;
	const float spaceBetweenEntries = 32f;
	const int maxElements = 10;

	[Export] private Texture2D emptySprite;
	[Export] private Texture2D airSprite;
	[Export] private Texture2D waterSprite;
	[Export] private Texture2D fireSprite;
	[Export] private Texture2D earthSprite;

	private Sprite2D[] overlayElements;
	private int firstEmptyIndex;

    // Signals
    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityUsed, new Callable(this, MethodName.RemoveLastElement));
        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityListUpdated, new Callable(this, MethodName.RefreshElements));
		
		overlayElements = new Sprite2D[maxElements];
		for (int i = 0; i < maxElements; i++)
		{
			Vector2 newPosition = Position;
			newPosition.X += i * spaceBetweenEntries;
			Sprite2D newSymbol = elementSymbol.Instantiate() as Sprite2D;
			newSymbol.Position = newPosition;

			overlayElements[i] = newSymbol;
			AddChild(newSymbol);
        }

		firstEmptyIndex = 0;
	}

    public void InsertElement(ElementState element)
	{
		overlayElements[firstEmptyIndex].Texture = ElementStateToSprite(element);
		firstEmptyIndex++;
	}

	public void RefreshElements(int[] elementArray)
	{
		int elListLen = elementArray.Length;
		for (int i = 0; i < maxElements; i++)
		{
			if (i < elListLen)
				overlayElements[i].Texture = ElementStateToSprite((ElementState)elementArray[i]);
			else
				overlayElements[i].Texture = emptySprite;
		}
		firstEmptyIndex = elListLen;
	}

	public void RemoveLastElement(int ability)
	{
		if (firstEmptyIndex > 0)
		{
			overlayElements[firstEmptyIndex - 1].Texture = emptySprite;
			firstEmptyIndex--;
		}
		else throw new ArgumentOutOfRangeException(); // There cannot be 0 children when removing one
	}

	public Texture2D ElementStateToSprite(ElementState element)
	{
		Texture2D elementSprite = element switch
		{
			ElementState.water => waterSprite,
			ElementState.air => airSprite,
			ElementState.fire => fireSprite,
			ElementState.earth => earthSprite,
			_ => waterSprite
		};
		return elementSprite;
	}
}
