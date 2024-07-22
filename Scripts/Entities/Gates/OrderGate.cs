using Godot;
using System;
using GlobalTypes;

public partial class OrderGate : Gate
{
    [Export] private int sequenceLength = 4;
    [Export] private ElementState ability0;
    [Export] private ElementState ability1;
    [Export] private ElementState ability2;
    [Export] private ElementState ability3;
    private ElementState[] abilityList;

    private int abilitiesCounted = 0;

    private ElementSymbol[] abilityDisplayList = new ElementSymbol[4];

    public override void _Ready()
	{
        base._Ready();

        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityUsed, new Callable(this, MethodName.CountAbility));
        abilityList = new[] { ability0, ability1, ability2, ability3 };

        for (int i = 0; i < sequenceLength; i++)
        {
            abilityDisplayList[i] = GetNode<AnimatedSprite2D>("ToMove/RequiredAbilities/Ability" + i.ToString()) as ElementSymbol;
            abilityDisplayList[i].ShowSprite(abilityList[i]);
        }

        abilityDisplayList[abilitiesCounted].ShowIndicator();
    }

    private void CountAbility(int ability)
    {
        if (isOpened) return;
        ElementState correctAbility = abilityList[abilitiesCounted];

        if (correctAbility == ElementState.normal || (ElementState)ability == correctAbility)
        {
            abilityDisplayList[abilitiesCounted].Modulate = new Color(1.0f, 1.0f, 1.0f, 0.5f); // lower opacity
            abilityDisplayList[abilitiesCounted].HideIndicator();
            abilitiesCounted++;

            if (abilitiesCounted == sequenceLength)
            {
                Open();
                return;
            }

            abilityDisplayList[abilitiesCounted].ShowIndicator();
        }
        else // reset
        {
            abilityDisplayList[abilitiesCounted].HideIndicator();
            abilitiesCounted = 0;
            foreach (ElementSymbol el in abilityDisplayList)
            {
                el.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            abilityDisplayList[abilitiesCounted].ShowIndicator();
        }       
    }
}
