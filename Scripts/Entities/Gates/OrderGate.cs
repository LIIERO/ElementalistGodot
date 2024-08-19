using Godot;
using System;
using GlobalTypes;
using System.Collections.Generic;

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

    // Undo system
    private List<int> orderGateCountCheckpoints = new List<int>();

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
            

            //abilityDisplayList[abilitiesCounted].Modulate = new Color(1.0f, 1.0f, 1.0f, 0.5f); // lower opacity
            abilityDisplayList[abilitiesCounted].Hide();
            abilityDisplayList[abilitiesCounted].HideIndicator();
            abilitiesCounted++;

            audioManager.orderGateSounds[abilitiesCounted].Play();

            if (abilitiesCounted == sequenceLength)
            {
                Open();
                return;
            }

            abilityDisplayList[abilitiesCounted].ShowIndicator();
        }
        else // reset
        {
            if (abilitiesCounted > 0) audioManager.orderGateSounds[0].Play();

            abilityDisplayList[abilitiesCounted].HideIndicator();
            abilitiesCounted = 0;
            for (int i = 0; i < sequenceLength; i++)
            {
                //abilityDisplayList[i].Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                abilityDisplayList[i].Show();
            }
            abilityDisplayList[abilitiesCounted].ShowIndicator();
        }       
    }

    private void SetGateCountState(int count)
    {
        for (int i = 0; i < sequenceLength; i++)
        {
            if (i < count)
            {
                abilityDisplayList[i].Hide();
                abilityDisplayList[i].HideIndicator();
            }
            else if (i == count)
            {
                abilityDisplayList[i].Show();
                abilityDisplayList[i].ShowIndicator();
            }
            else
            {
                abilityDisplayList[i].Show();
                abilityDisplayList[i].HideIndicator();
            }
        }
    }

    protected override void AddLocalCheckpoint()
    {
        base.AddLocalCheckpoint();
        orderGateCountCheckpoints.Add(abilitiesCounted);
    }

    protected override void UndoLocalCheckpoint(bool nextCpRequested)
    {
        base.UndoLocalCheckpoint(nextCpRequested);

        if (!nextCpRequested && orderGateCountCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(orderGateCountCheckpoints);
        abilitiesCounted = orderGateCountCheckpoints[^1];

        SetGateCountState(abilitiesCounted);
    }
}
