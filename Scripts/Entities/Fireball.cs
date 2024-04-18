using Godot;
using System;

public partial class Fireball : CharacterBody2D
{
    // singletons
    private GameState gameState;
    private AudioManager audioManager;

    [Export] Sprite2D spriteNode;
	const float speed = 200.0f;
    const float activationTime = 0.05f;
    //const float playerTeleportOffset = 2.0f;

    private float flyTime = 0.0f; 

	private float direction;


    // ECHO TRAIL STUFF
    //[Export] Node echoTrailSpawner;
    [Export] PackedScene echoTrailObject;
    const float timeBetweenSpawns = 0.02f;
    const float removalTime = 1.0f;
    float timeBetweenSpawnsCounter;


    public override void _Ready()
    {
        gameState = GetNode<GameState>("/root/GameState");
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;
        timeBetweenSpawnsCounter = timeBetweenSpawns;
    }

    public override void _PhysicsProcess(double delta)
	{
        flyTime += (float)delta;
        Velocity = new Vector2(direction * speed, 0.0f);

        if (flyTime > activationTime)
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

        var collision = MoveAndCollide(Velocity * (float)delta);
        if (collision != null)
        {
            QueueFree();

            if (flyTime > activationTime)
            {
                //Vector2 playerOffset = new(-direction * playerTeleportOffset, 0.0f);
                //gameState.SetPlayerPosition(GlobalPosition + playerOffset);
                audioManager.earthAbilityStart.Play(); // The earth start sound is the same as fireball hitting wall for now
                gameState.SetPlayerPosition(GlobalPosition);
            }
        }
    }

	public void SetDirection(bool right)
	{
        direction = right ? 1.0f : -1.0f;
		spriteNode.FlipH = !right;
    }

	void _OnVisibleOnScreenNotifier2dScreenExited()
	{
        GD.Print("Exited Screen");
        QueueFree();
    }

    private async void SpawnEchoObject()
    {
        Sprite2D instance = echoTrailObject.Instantiate() as Sprite2D;
        instance.FlipH = spriteNode.FlipH;
        GetTree().Root.GetChild(0).AddChild(instance);
        instance.GlobalPosition = GlobalPosition - new Vector2(0f, GameUtils.gameUnitSize);

        await ToSignal(GetTree().CreateTimer(removalTime, processInPhysics: true), "timeout");
        instance.QueueFree();
    }
}
