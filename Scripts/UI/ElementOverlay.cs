using Godot;
using System;
using GlobalTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

public partial class ElementOverlay : Node2D
{
	[Export] private PackedScene elementSymbol;
	const float spaceBetweenEntries = 32f;
	const int maxElements = 14;

	private ElementSymbol[] overlayElements;
	private int firstEmptyIndex;

    // Signals
    private CustomSignals customSignals;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityUsed, new Callable(this, MethodName.RemoveLastElement));
        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityListUpdated, new Callable(this, MethodName.RefreshElements));
		
		overlayElements = new ElementSymbol[maxElements];
		for (int i = 0; i < maxElements; i++)
		{
			Vector2 newPosition = Position;
			newPosition.X += i * spaceBetweenEntries;
            ElementSymbol newSymbol = elementSymbol.Instantiate() as ElementSymbol;
			newSymbol.Position = newPosition;

			overlayElements[i] = newSymbol;
			AddChild(newSymbol);
        }

		firstEmptyIndex = 0;
	}

	private void RefreshIndicator()
	{
		if (firstEmptyIndex > 0)
			overlayElements[firstEmptyIndex - 1].ShowIndicator();
		if (firstEmptyIndex > 1)
		{
			for (int i = 0; i < firstEmptyIndex - 1; i++)
			{
                overlayElements[i].HideIndicator();
            }
        } 
    }

    public void InsertElement(ElementState element)
	{
		overlayElements[firstEmptyIndex].ShowSprite(element);
		firstEmptyIndex++;

		RefreshIndicator();
	}

	public void RefreshElements(int[] elementArray)
	{
		int elListLen = elementArray.Length;
		for (int i = 0; i < maxElements; i++)
		{
			if (i < elListLen)
				overlayElements[i].ShowSprite((ElementState)elementArray[i]);
			else
				overlayElements[i].Hide();
		}
		firstEmptyIndex = elListLen;

		RefreshIndicator();
	}

	public void RemoveLastElement(int ability)
	{
		if (firstEmptyIndex > 0)
		{
			overlayElements[firstEmptyIndex - 1].Hide();
			firstEmptyIndex--;

			RefreshIndicator();
		}
		else throw new ArgumentOutOfRangeException(); // There cannot be 0 children when removing one
	}
}
