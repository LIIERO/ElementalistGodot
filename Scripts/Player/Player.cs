using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using GlobalTypes;

public partial class Player : CharacterBody2D
{
	// FOR TEMPORARY DEBUG RELOADING GAME SCENE
	// [Export] private Node2D gameScene;

	// Singletons
	private CustomSignals customSignals;
    //private GameState gameState;

    // Instaniates
    [Export] private PackedScene fireball;

    // Child nodes
    [Export] private AnimatedSprite2D animatedSprite;
	//[Export] private ShapeCast2D wallcheck;
	[Export] private Node spawner;
	[Export] private CpuParticles2D particles;
	private PlayerShaderEffects shaderScript;

	// Parameters
	public const float speed = 110.0f;
	public const float jumpVelocity = -220.0f;
	public const float maxFallingSpeed = 250f;
	public const float earthJumpPower = -275.0f;
	public const float dashPower = 220.0f;
	public const float dashTime = 0.2f;
	public const float waterJumpMomentumPreservation = 0.6f;
	public const float clingDrag = 0.7f;
	public const float coyoteTime = 0.1f;
	public const float inputBufferTime = 0.1f;
	public const float abilityFreezeTime = 0.05f;
	public const float jumpCancelFraction = 0.2f;
	public const float deathTime = 1.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float defaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float gravity;

	// Player state (move to a different class?)
	public List<ElementState> AbilityList { get; private set; }
	public ElementState BaseAbility { get; private set; } // Unused for now, Zoe can use this type of ability without orbs (standing on the ground refreshes it)

	ElementState currentAbility = ElementState.normal;
	bool isUsingAbility = false;
	bool isExecutingAbility = false;
	bool canUseBaseAbility = false;

    public bool IsFrozen { get; private set; } = false;
    public bool IsFacingRight { get; private set; } = true;
	public bool IsGrounded { get; private set; } = false;
	public bool IsClinging { get; private set; } = false;
	public bool isDying { get; private set; } = false;
	public bool IsHoldingGoal { get; set; } = false;
	//(bool left, bool right) huggingWall = (false, false); // useless I think
	float coyoteTimeCounter; float jumpBufferTimeCounter; float abilityBufferTimeCounter;

	// Interaction input
	public bool interactPressed = false;

	public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.SetPlayerPosition, new Callable(this, MethodName.SetPosition));
        //gameState = GetNode<GameState>("/root/GameState");

        gravity = defaultGravity;
		shaderScript = animatedSprite as PlayerShaderEffects;
		AbilityList = new List<ElementState>();
		BaseAbility = ElementState.normal;
	}

	public override void _PhysicsProcess(double delta)
	{
        Vector2 direction = Input.GetVector("inputLeft", "inputRight", "inputUp", "inputDown"); // Get the input direction
        bool restartPressed = Input.IsActionJustPressed("inputRestart"); // Reload scene button pressed
        interactPressed = Input.IsActionJustPressed("inputUp"); // Interact button pressed (handled by Interactable)
        bool jumpPressed = Input.IsActionJustPressed("inputJump");
        bool jumpReleased = Input.IsActionJustReleased("inputJump");
        bool abilityPressed = Input.IsActionJustPressed("inputAbility");

        IsGrounded = IsOnFloor();
		IsClinging = IsOnWallOnly() && Velocity.Y > 0f && ((!IsFacingRight && direction.X < -0.5f) || (IsFacingRight && direction.X > 0.5f));
        
		if (restartPressed) Die(deathTime);

		// Add the gravity
		if (!IsGrounded)
			Velocity += new Vector2(0.0f, gravity * (float)delta);

		Jump(jumpPressed, jumpReleased, delta);
		Ability(abilityPressed, delta);
		Movement(direction);

		UpdateAnimation(direction);
		if (!IsFrozen) MoveAndSlide();
	}

	// MOVEMENT ==================================================================================================================

	private void Movement(Vector2 direction)
	{
		if (isUsingAbility) return;
		
		// left right movement
		Velocity = new Vector2(direction.X * speed, Velocity.Y);

		// Falling speed cap
		if (Velocity.Y >= maxFallingSpeed)
			Velocity = new Vector2(Velocity.X, maxFallingSpeed);

		// resistance when clinging to a wall
		if (IsClinging)
			Velocity = new Vector2(Velocity.X, Velocity.Y * clingDrag);

	}


	private void Jump(bool jumpPressed, bool jumpReleased, double delta)
	{
		if (isUsingAbility) return;

		if (IsGrounded)
			coyoteTimeCounter = coyoteTime;
		else coyoteTimeCounter -= (float)delta;

		if (jumpPressed)
			jumpBufferTimeCounter = inputBufferTime;
		else jumpBufferTimeCounter -= (float)delta;

		if (jumpBufferTimeCounter > 0.0f && coyoteTimeCounter > 0.0f) // jump
		{
			// jump_sound.Play();
			// CreateJumpDust();
			Velocity = new Vector2(Velocity.X, jumpVelocity);
			jumpBufferTimeCounter = 0f;
		}

		if (Velocity.Y < 0.0f && jumpReleased) // cancel jump
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y * jumpCancelFraction);
			coyoteTimeCounter = 0f;
		}
		
	}


	// ABILITY ==================================================================================================================

	private void Ability(bool abilityPressed, double delta)
	{
		if (abilityPressed) abilityBufferTimeCounter = inputBufferTime;
		else abilityBufferTimeCounter -= (float)delta;

		// player uses ability
		if (!isUsingAbility && abilityBufferTimeCounter > 0f)
		{
			if (AbilityList.Count > 0 || (BaseAbility != ElementState.normal && canUseBaseAbility))
			{
				isUsingAbility = true;
				gravity = 0f;
				Velocity = new Vector2(0f, 0f);

				if (AbilityList.Count == 0)
				{
					canUseBaseAbility = false;
					currentAbility = BaseAbility;
				}
				else
				{
					// REMOVE TOP ABILITY AND INVOKE ABILITY USED EVENT
					currentAbility = AbilityList[^1];
					AbilityList.RemoveAt(AbilityList.Count - 1);
					customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityUsed, (int)currentAbility);
				}

				ExecuteAbilityAfterSeconds(abilityFreezeTime);
			}
		}

		if (isExecutingAbility)
		{

			if (currentAbility == ElementState.water)
			{
				Velocity = new Vector2(0f, -dashPower);
			}
			else if (currentAbility == ElementState.air)
			{
				if (IsFacingRight) Velocity = new Vector2(dashPower, 0f);
				else Velocity = new Vector2(-dashPower, 0f);
			}
			else if (currentAbility == ElementState.fire)
			{
				if (IsFacingRight) Velocity = new Vector2(-dashPower, 0f);
				else Velocity = new Vector2(dashPower, 0f);
			}
			else if (currentAbility == ElementState.earth)
			{
				if (IsGrounded)
				{
                    StopAbility();
                    SparkleAbilityDust(ElementState.earth, 0.1f);
                    abilityBufferTimeCounter = 0f; // Preventing overlapping abilities
					Velocity = new Vector2(Velocity.X, earthJumpPower);
				}
				else Velocity = new Vector2(0f, dashPower);
			}
		}

		else if (IsGrounded) canUseBaseAbility = true;
	}

	async private void ExecuteAbilityAfterSeconds(float t)
	{
		await ToSignal(GetTree().CreateTimer(t), "timeout");

		//abilitySound.Play();

		isExecutingAbility = true;
		StartAbilityDust(currentAbility);
		shaderScript.ActivateTrail(currentAbility);

		if (currentAbility == ElementState.air)
		{
			await ToSignal(GetTree().CreateTimer(dashTime), "timeout");
			StopAbility();
		}
		else if (currentAbility == ElementState.water)
		{
			await ToSignal(GetTree().CreateTimer(dashTime / 2f), "timeout");
			StopAbility();
		}
		else if (currentAbility == ElementState.fire)
		{
			SpawnFireball();
			await ToSignal(GetTree().CreateTimer(dashTime / 2f), "timeout");
			StopAbility();
		}
	}

	private void StopAbility()
	{
		shaderScript.UpdatePlayerColor(GetEffectiveElement());
		StopAbilityDust();
		shaderScript.DeactivateTrail();

		isUsingAbility = false;
		isExecutingAbility = false;
		currentAbility = ElementState.normal;
		gravity = defaultGravity;
		Velocity = new Vector2(0f, Velocity.Y * waterJumpMomentumPreservation);
		coyoteTimeCounter = 0f;
		jumpBufferTimeCounter = 0f;
	}

	private void SpawnFireball()
	{
		const float downOffset = -9f;

        Node2D instance = fireball.Instantiate() as Node2D;
		(instance as Fireball).SetDirection(IsFacingRight);
        spawner.AddChild(instance);
        instance.GlobalPosition = GlobalPosition + new Vector2(0.0f, downOffset);
	}

    public void StartAbilityDust(ElementState elementState)
    {
		particles.Color = GameUtils.ColorsetToColor[GameUtils.ElementToColorset[elementState]];
		particles.Emitting = true;
    }

    public void StopAbilityDust()
    {
        particles.Emitting = false;
    }

    async private void SparkleAbilityDust(ElementState elementState, float time)
    {
        StartAbilityDust(elementState);
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        StopAbilityDust();
    }

    // ANIMATION ==================================================================================================================

    private void UpdateAnimation(Vector2 direction)
	{
		if (isDying) return; // Death animation set in Die method

		// change direction facing
		if (!isExecutingAbility) // you can change direction after you started ability before it executed for input leniency
		{
			if (IsFacingRight && direction.X < -0.5f)
			{
				IsFacingRight = false;
				animatedSprite.FlipH = true;
			}
			else if (!IsFacingRight && direction.X > 0.5f)
			{
				IsFacingRight = true;
				animatedSprite.FlipH = false;
			}
		}

		// animations
		string animation;

		if (currentAbility == ElementState.air || currentAbility == ElementState.water || currentAbility == ElementState.earth)
			animation = "Dash";
		else if (currentAbility == ElementState.fire)
			animation = "Fireball";
		else if (IsGrounded)
		{
			if (direction.X == 0f || IsOnWall())
				animation = "Idle";
			else animation = "Run";
		}
		else
		{
			if (Velocity.Y < -0.1f) animation = "Jump";
			else if (IsClinging) animation = "Cling";
			else animation = "Fall";
		}

		if (GameUtils.PlayingCutscene)
		{
			animation = "Idle";
			Velocity = new Vector2(0f, 0f);
		}

		animatedSprite.Play(animation);
	}


    // DYING RESPAWNING

    async void Die(float t)
	{
		//if (isUsingAbility) return;
		//Global.PlayingCutscene = true;
		isDying = true;
		IsFrozen = true; // player floats when killed
        animatedSprite.Play("Die"); // death animation
        await ToSignal(GetTree().CreateTimer(t), "timeout");
		GetTree().ReloadCurrentScene(); // TEMPORARY
	}

	// PUBLIC ==================================================================================================================

	public ElementState GetEffectiveElement()
	{
		ElementState effectiveElem;
		if (AbilityList.Count == 0) { effectiveElem = BaseAbility; }
		else { effectiveElem = AbilityList[^1]; }
		return effectiveElem;
	}

	public void SetPosition(Vector2 position)
	{
		GlobalPosition = position;
	}

}
