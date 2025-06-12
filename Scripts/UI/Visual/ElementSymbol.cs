using Godot;
using System;
using GlobalTypes;

public partial class ElementSymbol : AnimatedSprite2D
{
	private AnimatedSprite2D currentSymbolIndicator;

	public override void _Ready()
	{
		currentSymbolIndicator = GetNode<AnimatedSprite2D>("Indicator");
        currentSymbolIndicator.Hide();
        Hide();
	}

    public void ShowSprite(ElementState element)
    {
        switch (element)
		{
			case ElementState.water:
                Show(); Play("Water"); break;
            case ElementState.air:
                Show(); Play("Air"); break;
            case ElementState.earth:
                Show(); Play("Earth"); break;
            case ElementState.fire:
                Show(); Play("Fire"); break;
            case ElementState.love:
                Show(); Play("Love"); break;
            case ElementState.normal:
                Show(); Play("Normal"); break;
            case ElementState.earth_remix:
                Show(); Play("Earth"); break;
            default:
                Hide(); break;   
        }
    }

    public void ShowIndicator()
    {
        currentSymbolIndicator.Show();
        currentSymbolIndicator.Play("default");
    }

    public void HideIndicator()
    {
        currentSymbolIndicator.Stop();
        currentSymbolIndicator.Hide();
    }
}
