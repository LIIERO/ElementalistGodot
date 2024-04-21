using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using GlobalTypes;
using System.Reflection.Metadata;

public partial class Player : CharacterBody2D
{
    // Code is not good but I don't really care
    // This whole script is me stubbornly refusing to implement a state machine for the player

    // Singletons
    private CustomSignals customSignals;
    private GameState gameState;
    private LevelTransitions levelTransitions;
    private AudioManager audioManager;

    // Instantiates
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
	public const float dashPower = 230.0f;
	public const float dashTime = 0.2f;
	public const float waterJumpMomentumPreservation = 0.6f;
	public const float clingDrag = 0.8f;
	public const float coyoteTime = 0.1f;
	public const float inputBufferTime = 0.1f;
	public const float abilityFreezeTime = 0.05f;
	public const float jumpCancelFraction = 0.2f;
	public const float deathTime = 0.2f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float defaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float gravity;

	// Player state (move to a different class?)
	public List<ElementState> AbilityList { get; private set; }
	public ElementState BaseAbility { get; private set; } // Unused for now, Zoe can use this type of ability without orbs (standing on the ground refreshes it)
    public bool IsHoldingGoal { get; set; } = false;
	public bool IsFrozen => isDying || gameState.IsLevelTransitionPlaying;

    ElementState currentAbility = ElementState.normal;
	public bool isUsingAbility = false;
	bool isExecutingAbility = false;
	bool canUseBaseAbility = false;
    public bool isFacingRight = true;
	public bool isGrounded = false;
	public bool isClinging = false;
	public bool isDying = false;
	public bool canJumpCancel = true;
	float coyoteTimeCounter; float jumpBufferTimeCounter; float abilityBufferTimeCounter;

	[Export] private Timer abilityTimer = null;
	//private float timeLeftAfterPause = 0.0f;

    private string currentAnimation;
	private float footstepTimer = 0.0f;
	private float jumpPreventionTimer = -0.01f; // for teleport bug
	private bool particlesActive;
	private bool wallslideSlowdownActive;

    // Input
    float direction; // input direction
    bool restartPressed; // Reload scene button pressed
    bool jumpPressed;
    bool jumpReleased;
    bool abilityPressed;

	private static bool setPlayerRespawnPosition = false; // Flag that adjusts player respawn position after restarting in hub

    public override void _Ready()
	{
		customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.SetPlayerPosition, new Callable(this, MethodName.SetPosition));
        //customSignals.Connect(CustomSignals.SignalName.GamePaused, new Callable(this, MethodName.PauseAbilityTimer));
        gameState = GetNode<GameState>("/root/GameState");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

		particlesActive = GetNode<SettingsManager>("/root/SettingsManager").LightParticlesActive;
		if (!particlesActive) particles.QueueFree();
		wallslideSlowdownActive = GetNode<SettingsManager>("/root/SettingsManager").WallslideSlowdownActive;

        gravity = defaultGravity;
		shaderScript = animatedSprite as PlayerShaderEffects;
		AbilityList = new List<ElementState>();
		BaseAbility = ElementState.normal;

		if (setPlayerRespawnPosition == true) // Respawning after dying in a Hub (multiple checkpoints)
        {
            setPlayerRespawnPosition = false;
			SetPosition(gameState.PlayerHubRespawnPosition);
        }
    }

    public override void _PhysicsProcess(double delta)
	{
        // Get input
        direction = Input.GetAxis("inputLeft", "inputRight");
        restartPressed = Input.IsActionJustPressed("inputRestart");
        jumpPressed = Input.IsActionJustPressed("inputJump");
        jumpReleased = Input.IsActionJustReleased("inputJump");
        abilityPressed = Input.IsActionJustPressed("inputAbility");

		// Pressed interact button
		if (Input.IsActionJustPressed("inputUp") && !IsFrozen) customSignals.EmitSignal(CustomSignals.SignalName.PlayerInteracted);

        isGrounded = IsOnFloor();
		isClinging = IsOnWallOnly() && Velocity.Y > 0.001f && ((!isFacingRight && direction < 0.0f) || (isFacingRight && direction > 0.0f));
        
		if (restartPressed && !IsFrozen) Kill(); // Retry level

		// Add the gravity
		if (!isGrounded)
			Velocity += new Vector2(0.0f, gravity * (float)delta);

		Jump(jumpPressed, jumpReleased, delta);
		Ability(abilityPressed, delta);
		Movement(direction);

		if (!IsFrozen)
		{
            MoveAndSlide();
            CheckForCollision(delta);
        }
        UpdateAnimation(direction);
    }

	// MOVEMENT ==================================================================================================================

	private void Movement(float direction)
	{
		if (isUsingAbility) return;
		
		// left right movement
		Velocity = new Vector2(direction * speed, Velocity.Y);

		// Falling speed cap
		if (Velocity.Y >= maxFallingSpeed)
			Velocity = new Vector2(Velocity.X, maxFallingSpeed);

		// resistance when clinging to a wall
		if (isClinging && wallslideSlowdownActive)
			Velocity = new Vector2(Velocity.X, Velocity.Y * clingDrag);
	}


	private void Jump(bool jumpPressed, bool jumpReleased, double delta)
	{
		if (jumpPreventionTimer > 0.0f)
		{
			Velocity = new Vector2(Velocity.X, 0.0f); // Fix of a very obscure earth ability tech
			jumpPreventionTimer -= (float)delta;
		}
        if (isUsingAbility || jumpPreventionTimer > 0.0f) return;

        if (isGrounded)
		{
            coyoteTimeCounter = coyoteTime;
            canJumpCancel = true;
        }
		else coyoteTimeCounter -= (float)delta;

		if (jumpPressed)
			jumpBufferTimeCounter = inputBufferTime;
		else jumpBufferTimeCounter -= (float)delta;

		if (jumpBufferTimeCounter > 0.0f && coyoteTimeCounter > 0.0f) // jump
		{
			//audioManager.softFootsteps[audioManager.softFootsteps.Length - 1].Play();
			// CreateJumpDust();
			Velocity = new Vector2(Velocity.X, jumpVelocity);
			jumpBufferTimeCounter = 0f;
			footstepTimer = -0.1f; // random negative number so player makes sound falling on the ground
		}

		if (canJumpCancel && Velocity.Y < 0.0f && jumpReleased) // cancel jump
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
		if (!isUsingAbility && !IsFrozen && abilityBufferTimeCounter > 0f)
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
				if (isFacingRight) Velocity = new Vector2(dashPower, 0f);
				else Velocity = new Vector2(-dashPower, 0f);
			}
			else if (currentAbility == ElementState.fire)
			{
				if (isFacingRight) Velocity = new Vector2(-dashPower/2f, 0f);
				else Velocity = new Vector2(dashPower/2f, 0f);
			}
			else if (currentAbility == ElementState.earth)
			{
				if (isGrounded)
				{
                    StopAbility();
                    SparkleAbilityDust(ElementState.earth, 0.1f);
                    abilityBufferTimeCounter = 0f; // Preventing overlapping abilities
					isGrounded = false;
					Velocity = new Vector2(Velocity.X, earthJumpPower);
					audioManager.earthAbilityEnd.Play();
				}
				else Velocity = new Vector2(0f, dashPower);
			}
		}

		else if (isGrounded) canUseBaseAbility = true;
	}

	async private void ExecuteAbilityAfterSeconds(float t)
	{
        await ToSignal(GetTree().CreateTimer(t, processInPhysics: true), "timeout");

        isExecutingAbility = true;
		StartAbilityDust(currentAbility);
		shaderScript.ActivateTrail(currentAbility);

		if (currentAbility == ElementState.air)
		{
			audioManager.airAbility.Play();

			abilityTimer.Start(dashTime);
			await ToSignal(abilityTimer, "timeout");
            abilityTimer.Stop();

            StopAbility();
		}
		else if (currentAbility == ElementState.water)
		{
			audioManager.waterAbility.Play();

            abilityTimer.Start(dashTime / 2f);
            await ToSignal(abilityTimer, "timeout");
			abilityTimer.Stop();

            jumpPreventionTimer = -0.1f; // So your momentum doesnt get lost after teleporting
            StopAbility();
		}
		else if (currentAbility == ElementState.fire)
		{
            audioManager.fireAbility.Play();
            SpawnFireball();
			SparkleAbilityDust(ElementState.fire, 0.1f);
			abilityBufferTimeCounter = -0.1f; // So it doesn't trigger twice

            abilityTimer.Start(dashTime / 2f);
            await ToSignal(abilityTimer, "timeout");
            abilityTimer.Stop();

            StopAbility();
		}
		else if (currentAbility == ElementState.earth)
		{
			audioManager.earthAbilityStart.Play();
		}
		else GD.Print("Execute ability broke very badly.");
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
		canJumpCancel = false;
	}

    private void SpawnFireball()
	{
		float rightOffset = isFacingRight ? 9f : -9f;

        Node2D instance = fireball.Instantiate() as Node2D;
		(instance as Fireball).SetDirection(isFacingRight);
        spawner.AddChild(instance);
        instance.GlobalPosition = GlobalPosition + new Vector2(rightOffset, 0.0f);
	}

    public void StartAbilityDust(ElementState elementState)
    {
		if (!particlesActive) return;
		particles.Color = GameUtils.ColorsetToColor[GameUtils.ElementToColorset[elementState]];
		particles.Emitting = true;
    }

    public void StopAbilityDust()
    {
		if (!particlesActive) return;
        particles.Emitting = false;
    }

    async private void SparkleAbilityDust(ElementState elementState, float time)
    {
        StartAbilityDust(elementState);
        await ToSignal(GetTree().CreateTimer(time, processInPhysics: true), "timeout");
        StopAbilityDust();
    }

    // ANIMATION ==================================================================================================================

    private void UpdateAnimation(float direction)
	{
		if (gameState.IsLevelTransitionPlaying && !isDying) animatedSprite.Play("Idle"); // Level transition animation
        if (IsFrozen) return;

		// change direction facing
		if (!isExecutingAbility) // you can change direction after you started ability before it executed for input leniency
		{
			if (isFacingRight && direction < 0.0f)
			{
				isFacingRight = false;
				animatedSprite.FlipH = true;
			}
			else if (!isFacingRight && direction > 0.0f)
			{
				isFacingRight = true;
				animatedSprite.FlipH = false;
			}
		}

		// animations
		if (currentAbility == ElementState.air || currentAbility == ElementState.water || currentAbility == ElementState.earth)
			currentAnimation = "Dash";
		else if (currentAbility == ElementState.fire)
			currentAnimation = "Fireball";
		else if (isGrounded)
		{
			if (direction == 0.0f || IsOnWall())
				currentAnimation = "Idle";
			else currentAnimation = "Run";
		}
		else
		{
			if (Velocity.Y < -1.0f) currentAnimation = "Jump";
			else if (isClinging) currentAnimation = "Cling";
			else if (Velocity.Y > 1.0f) currentAnimation = "Fall";
			else currentAnimation = "Idle";
        }

		animatedSprite.Play(currentAnimation);
	}


    // DYING

    void _OnArea2dBodyEntered(Node2D body) // This is a small area in the player, if a body enters it the player got crushed
	{
		if (body is not Player && body is not Fireball)
			Kill();
	}

    async void Die(float t)
	{
        if (gameState.IsHubLoaded() && gameState.PlayerHubRespawnPosition != Vector2.Inf) // Dying in Hub (multiple checkpoints)
            setPlayerRespawnPosition = true;

        customSignals.EmitSignal(CustomSignals.SignalName.PlayerDied);
        isDying = true;
        animatedSprite.Play("Die"); // death animation
		audioManager.death.Play();
        await ToSignal(GetTree().CreateTimer(t, processInPhysics: true), "timeout");
        levelTransitions.StartLevelReloadTransition();
    }

    // COLLISIONS

    private void CheckForCollision(double delta)
	{
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            Node collider = collision.GetCollider() as Node;

            if (collider.IsInGroup("Danger"))
			{
				Kill();
				break;
			}

			// Making proper sound when walking on something
            if (collider.IsInGroup("PlayerCollider"))
            {
                float angle = collision.GetAngle();
                if (angle > 0.001f || angle < -0.001f) continue; // Check if is touching floor

                if (currentAnimation == "Run") footstepTimer -= (float)delta; // if running count down to the next sound
				if (footstepTimer >= 0.0f) continue;
				footstepTimer = 0.2f;

				if (collider.IsInGroup("SoftMaterial"))
					audioManager.PlayRandomSound(audioManager.softFootsteps);
                else if (collider.IsInGroup("HardMaterial"))
                    audioManager.PlayRandomSound(audioManager.hardFootsteps);
            }
        }
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
        //CancelAbility(); // Decided it was unnecessary

        jumpPreventionTimer = abilityFreezeTime;
        coyoteTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        canJumpCancel = false;
		isGrounded = false;
		Velocity = new Vector2(0f, 0f);

        GlobalPosition = position;
	}

    public void Kill()
    {
        Die(deathTime);
    }
}
