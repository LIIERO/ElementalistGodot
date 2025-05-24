using Godot;
using System.Collections.Generic;
using GlobalTypes;

public abstract partial class Orb : Area2D, IUndoable
{
	[Export] public ColorSet refillColor;
	private Light2D backgroundLight;
    private CpuParticles2D particles;

    // Singletons
    private AudioManager audioManager;
    private CustomSignals customSignals;

    // Undo system
    private bool isActive = true;
    private List<bool> orbStateCheckpoints = new List<bool>();

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.ReplaceTopCheckpoint, new Callable(this, MethodName.ReplaceTopLocalCheckpoint));

        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        backgroundLight = GetNode<PointLight2D>("MovedByAnimation/PointLight2D");
        particles = GetNode<CpuParticles2D>("MovedByAnimation/GPUParticles2D");

        bool lightParticlesActive = GetNode<SettingsManager>("/root/SettingsManager").LightParticlesActive;
        if (!lightParticlesActive)
        {
            backgroundLight.QueueFree();
            particles.QueueFree();
        }
        else
        {
            Color color = GameUtils.ColorsetToColor[refillColor];
            particles.Color = color;
            backgroundLight.Color = color;
            if (refillColor == ColorSet.white)
                backgroundLight.Energy *= 0.75f;
            if (refillColor == ColorSet.black)
            {
                backgroundLight.BlendMode = Light2D.BlendModeEnum.Sub;
                backgroundLight.Energy *= 1.5f;
            }
                
        }
    }

	protected virtual void ModifyElementStack(List<ElementState> elementStack) {}

	void _OnBodyEntered(Node2D body)
	{
        if (!isActive) return;
        if (body is not Player) return;

        Disable();
        //QueueFree();

        // Get the script of player node and modify its ability list
        Player playerScript = body as Player;
        audioManager.orbCollectSound.Play();
		ModifyElementStack(playerScript.AbilityList);
        customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityListUpdated, GameUtils.ElementListToIntArray(playerScript.AbilityList));

        customSignals.EmitSignal(CustomSignals.SignalName.RequestCheckpoint);
	}

    public void AddLocalCheckpoint()
    {
        orbStateCheckpoints.Add(isActive);
    }

    public void UndoLocalCheckpoint(bool nextCpRequested)
    {
        if (!nextCpRequested && orbStateCheckpoints.Count > 1) GameUtils.ListRemoveLastElement(orbStateCheckpoints);
        isActive = orbStateCheckpoints[^1];
        
        if (isActive) Enable();
        else Disable();
    }

    public void ReplaceTopLocalCheckpoint()
    {
        GameUtils.ListRemoveLastElement(orbStateCheckpoints);
        AddLocalCheckpoint();
    }

    private void Enable()
    {
        isActive = true;
        Show();
    }

    private void Disable()
    {
        isActive = false;
        Hide();
    }

}
