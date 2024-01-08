using Godot;
using System;
using System.Collections.Generic;
using static GlobalTypes;

public partial class PlayerShaderEffects : AnimatedSprite2D
{
    ShaderMaterial shaderMaterial;

    Vector3 greenShirt = new(0.16f, 0.8f, 0.16f);
    Vector3 greenSleeves = new(0.12f, 0.6f, 0.12f);
    Vector3 greenParticle = new(0.5f, 1.0f, 0.5f);

    Vector3 blueShirt = new(0.16f, 0.16f, 0.8f);
    Vector3 blueSleeves = new(0.12f, 0.12f, 0.6f);
    Vector3 blueParticle = new(0.5f, 0.5f, 1.0f);

    Vector3 redShirt = new(0.8f, 0.16f, 0.16f);
    Vector3 redSleeves = new(0.6f, 0.12f, 0.12f);
    Vector3 redParticle = new(1.0f, 0.5f, 0.5f);

    Vector3 whiteShirt = new(0.8f, 0.8f, 0.8f);
    Vector3 whiteSleeves = new(0.6f, 0.6f, 0.6f);

    Vector3 brownShirt = new(0.588f, 0.353f, 0.118f);
    Vector3 brownSleeves = new(0.471f, 0.282f, 0.094f);
    Vector3 brownParticle = new(1.0f, 0.5f, 0.0f);

    // Signals
    private CustomSignals customSignals;

    public override void _Ready()
	{
        shaderMaterial = Material as ShaderMaterial;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.PlayerAbilityListUpdated, new Callable(this, MethodName.UpdatePlayerColorFromArray));
    }

    public void UpdatePlayerColor(ElementState element)
    {
        switch (element)
        {
            case ElementState.water:
                shaderMaterial.SetShaderParameter("shirtColor", blueShirt);
                shaderMaterial.SetShaderParameter("sleeveColor", blueSleeves); break;
            case ElementState.fire:
                shaderMaterial.SetShaderParameter("shirtColor", redShirt);
                shaderMaterial.SetShaderParameter("sleeveColor", redSleeves); break;
            case ElementState.air:
                shaderMaterial.SetShaderParameter("shirtColor", whiteShirt);
                shaderMaterial.SetShaderParameter("sleeveColor", whiteSleeves); break;
            case ElementState.earth:
                shaderMaterial.SetShaderParameter("shirtColor", brownShirt);
                shaderMaterial.SetShaderParameter("sleeveColor", brownSleeves); break;
            default:
                shaderMaterial.SetShaderParameter("shirtColor", greenShirt);
                shaderMaterial.SetShaderParameter("sleeveColor", greenSleeves); break;
        }
    }

    public void UpdatePlayerColorFromArray(int[] elementArray)
    {
        if (elementArray.Length > 0)
        {
            UpdatePlayerColor((ElementState)elementArray[^1]);
        }
    }



    // ECHO TRAIL

    [Export] Node echoTrailSpawner;
    [Export] PackedScene echoTrailObject;
    const float timeBetweenSpawns = 0.02f;
    const float removalTime = 2.0f;

    [Export] Texture2D waterAbility;
    [Export] Texture2D airAbility;
    [Export] Texture2D earthAbility;
    [Export] Texture2D fireAbility;

    bool trailActive;
    float timeBetweenSpawnsCounter;
    Texture2D trailSprite;


    public override void _Process(double delta)
    {
        if (trailActive)
        {
            if (timeBetweenSpawnsCounter <= 0)
            {
                SpawnEchoObject();
                timeBetweenSpawnsCounter = timeBetweenSpawns;
            }
            else
            {
                timeBetweenSpawnsCounter -= (float)delta;
            }
        }
    }

    private async void SpawnEchoObject()
    {
        Sprite2D instance = echoTrailObject.Instantiate() as Sprite2D;
        instance.FlipH = FlipH;
        instance.Texture = trailSprite;
        echoTrailSpawner.AddChild(instance);
        instance.GlobalPosition = GlobalPosition;
        //RemoveChild(instance);

        await ToSignal(GetTree().CreateTimer(removalTime), "timeout");
        instance.QueueFree();
    }

    public void ActivateTrail(ElementState abilityElement)
    {
        trailActive = true;
        timeBetweenSpawnsCounter = timeBetweenSpawns;

        trailSprite = abilityElement switch
        {
            ElementState.water => waterAbility,
            ElementState.air => airAbility,
            ElementState.earth => earthAbility,
            ElementState.fire => fireAbility,
            _ => throw new NotImplementedException(),
        };
    }

    public void DeactivateTrail()
    {
        trailActive = false;
    }
}
