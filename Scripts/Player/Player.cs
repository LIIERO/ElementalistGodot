using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using static GlobalTypes;

public partial class Player : CharacterBody2D
{
	// FOR TEMPORARY DEBUG RELOADING GAME SCENE
	// [Export] private Node2D gameScene;

	// Signals
	private CustomSignals customSignals;

    // Child nodes
    [Export] private AnimatedSprite2D animatedSprite;
	[Export] private ShapeCast2D wallcheck;
	private PlayerShader shaderScript;

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

	bool isFrozen = false;
	ElementState currentAbility = ElementState.normal;
	bool isUsingAbility = false;
	bool isExecutingAbility = false;
	bool canUseBaseAbility = false;
	bool isFacingRight = true;
	bool isGrounded = false;
	//(bool left, bool right) huggingWall = (false, false); // useless I think
	float coyoteTimeCounter; float jumpBufferTimeCounter; float abilityBufferTimeCounter;
	

	public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");

		gravity = defaultGravity;
		shaderScript = animatedSprite as PlayerShader;
		AbilityList = new List<ElementState>();
		BaseAbility = ElementState.normal;
	}

	public override void _PhysicsProcess(double delta)
	{
		isGrounded = IsOnFloor();

		// Reload scene
		bool restartPressed = Input.IsActionJustPressed("inputRestart");
		if (restartPressed) Die(deathTime);

		// Add the gravity
		if (!isGrounded)
			Velocity += new Vector2(0.0f, gravity * (float)delta);

		// Handle Jump
		bool jumpPressed = Input.IsActionJustPressed("inputJump");
		bool jumpReleased = Input.IsActionJustReleased("inputJump");
		Jump(jumpPressed, jumpReleased, delta);

		// Use ability
		bool abilityPressed = Input.IsActionJustPressed("inputAbility");
		Ability(abilityPressed, delta);

		// Get the input direction and handle the movement/deceleration
		// As good practice, you should replace UI actions with custom gameplay actions
		Vector2 direction = Input.GetVector("inputLeft", "inputRight", "inputUp", "inputDown");
		Movement(direction);

		UpdateAnimation(direction);
		if (!isFrozen) MoveAndSlide();
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
		if (IsOnWallOnly() && Velocity.Y > 0f)
			Velocity = new Vector2(Velocity.X, Velocity.Y * clingDrag);

	}


	private void Jump(bool jumpPressed, bool jumpReleased, double delta)
	{
		if (isUsingAbility) return;

		if (isGrounded)
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

	void Ability(bool abilityPressed, double delta)
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
					//AbilityUsed?.Invoke(currentAbility);
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
				if (isFacingRight) Velocity = new Vector2(dashPower, 0f);
				else Velocity = new Vector2(-dashPower, 0f);
			}
			else if (currentAbility == ElementState.fire)
			{
				if (isFacingRight) Velocity = new Vector2(-dashPower, 0f);
				else Velocity = new Vector2(dashPower, 0f);
			}
			else if (currentAbility == ElementState.earth)
			{
				if (isGrounded)
				{
					StopAbility();
					//StartCoroutine(AdditionalDustSparkleForGroundAbility());
					abilityBufferTimeCounter = 0f; // Preventing overlapping abilities
					Velocity = new Vector2(Velocity.X, earthJumpPower);
				}
				else Velocity = new Vector2(0f, dashPower);
			}
		}

		else if (isGrounded) canUseBaseAbility = true;
	}

	async void ExecuteAbilityAfterSeconds(float t)
	{
		await ToSignal(GetTree().CreateTimer(t), "timeout");

		//abilitySound.Play();

		isExecutingAbility = true;
		//StartAbilityDust(currentAbility);
		//trailScript.ActivateTrail(currentAbility);

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
			//SpawnFireball();
			await ToSignal(GetTree().CreateTimer(dashTime / 2f), "timeout");
			StopAbility();
		}
	}

	void StopAbility()
	{
		shaderScript.UpdatePlayerColor(GetZoeEffectiveElement());
		// StopAbilityDust();
		// trailScript.DeactivateTrail();

		isUsingAbility = false;
		isExecutingAbility = false;
		currentAbility = ElementState.normal;
		gravity = defaultGravity;
		Velocity = new Vector2(0f, Velocity.Y * waterJumpMomentumPreservation);
		coyoteTimeCounter = 0f;
		jumpBufferTimeCounter = 0f;
	}

	/*void SpawnFireball()
	{
		float downOffset = 0.4f;
		GameObject newFireball = Instantiate(fireball, transform.position - new Vector3(0f, downOffset, 0f), transform.rotation);
		if (!isFacingRight) newFireball.transform.localScale = new Vector3(-1, 1, 1);
	}*/

	// ANIMATION ==================================================================================================================

	private void UpdateAnimation(Vector2 direction)
	{
		// change direction facing
		if (!isExecutingAbility) // you can change direction after you started ability before it executed for input leniency
		{
			if ((isFacingRight && direction.X < 0f) || (IsOnWallOnly() && !wallcheck.IsColliding()))
			{
				isFacingRight = false;
				animatedSprite.FlipH = true;
			}
			else if ((!isFacingRight && direction.X > 0f) || (IsOnWallOnly() && wallcheck.IsColliding()))
			{
				isFacingRight = true;
				animatedSprite.FlipH = false;
			}
		}

		// animations
		string animation;

		if (currentAbility == ElementState.air || currentAbility == ElementState.water || currentAbility == ElementState.earth)
			animation = "Dash";
		else if (currentAbility == ElementState.fire)
			animation = "Fireball";
		else if (isGrounded)
		{
			if (direction.X == 0f || IsOnWall())
				animation = "Idle";
			else animation = "Run";
		}
		else
		{
			if (Velocity.Y < -0.1f) animation = "Jump";
			else if (IsOnWallOnly()) animation = "Cling";
			else animation = "Fall";
		}

		if (GlobalUtils.PlayingCutscene)
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
		isFrozen = true; // player floats when killed
		//anim.SetTrigger("death"); // death animation
		await ToSignal(GetTree().CreateTimer(t), "timeout");
		GetTree().ReloadCurrentScene(); // TEMPORARY
	}

	// PUBLIC ==================================================================================================================

	public ElementState GetZoeEffectiveElement()
	{
		ElementState effectiveElem;
		if (AbilityList.Count == 0) { effectiveElem = BaseAbility; }
		else { effectiveElem = AbilityList[^1]; }
		return effectiveElem;
	}

}
