using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using GlobalTypes;
using System.Reflection.Metadata;
using System.Net.Http.Headers;
using System.Diagnostics;

public partial class Player : CharacterBody2D, IUndoable
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
	[Export] private CollisionShape2D hitbox;
	[Export] private Node spawner;
	[Export] private CpuParticles2D particles;
	[Export] private Sprite2D checkpointIndicator;
	[Export] private AnimationPlayer propertyAnimations;
	private PlayerShaderEffects shaderScript;

	// Parameters
	public const float speed = 109.0f;
	public const float jumpVelocity = -220.0f;
	public const float maxFallingSpeed = 250f;
	public const float earthJumpPower = -275.0f;
	public const float dashPower = 230.0f;
	public const float dashTime = 0.2f;
	public const float waterJumpMomentumPreservation = 0.55f;
	public const float clingDrag = 0.8f;
	public const float coyoteTime = 0.1f;
	public const float inputBufferTime = 0.1f; // Multi purposed for a plenty of tiny counters
	public const float abilityFreezeTime = 0.05f;
	public const float jumpCancelFraction = 0.2f;
	public const float jumpPreventionTime = 0.15f;
	public const float deathTime = 0.2f;
	public const float floatyGravityMaxVelocity = 50.0f;
	public const int windDashCornerCorrection = 5;
	public const int verticalCornerCorrection = 4;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float defaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float gravity;

	// Player state (move to a different class?)
	public List<ElementState> AbilityList { get; private set; }
	public ElementState BaseAbility { get; private set; } // Unused for now, Zoe can use this type of ability without orbs (standing on the ground refreshes it)
    public bool IsHoldingGoal { get; set; } = false; // Yellow or red
    public bool IsHoldingSpecialGoal { get; set; } = false; // Red
	public bool SqueezeDeathDisabled { get; set; } = false; // Immune from a gate crush death
    public bool IsFrozen => isDead || isUndoing || gameState.IsLevelTransitionPlaying || gameState.IsDialogActive || gameState.WatchtowerActive;

    ElementState currentAbility = ElementState.normal; // Different from normal only while using it
	public bool isUsingAbility = false;
	bool isExecutingAbility = false;
	bool canUseBaseAbility = false;
    public bool isFacingRight = true;
	public bool isGrounded = false;
	public bool isClinging = false;
	public bool isDead = false;
	public bool canJumpCancel = true;
	public bool isUndoing = false;
	//public bool isAddingCheckpoint = false;
	public float checkpointTimer = -0.1f;
	public bool isOnMovingEntity = false; // For now only true when standing on a gate that is moving up
	float coyoteTimeCounter; float jumpBufferTimeCounter; float abilityBufferTimeCounter;

	[Export] private Timer abilityTimer = null;
	private SceneTreeTimer addAbilityTimer = null;
	//private float timeLeftAfterPause = 0.0f;

    private string currentAnimation;
	private float footstepTimer = 0.0f;
	private float jumpMovePreventionTimer = -0.1f; // for teleport bug
	private bool particlesActive;
	private bool wallslideSlowdownActive;

    // Input
    float direction; // input direction
    bool restartPressed; // Reload scene button pressed
    bool jumpPressed;
    bool jumpReleased;
    bool abilityPressed;
	bool undoPressed;

	public static bool setPlayerRespawnPosition = false; // Flag that adjusts player respawn position after restarting in hub

	// Undo system
	private bool checkpointRequested = false;
	private List<Vector2> playerPositionCheckpoints = new List<Vector2>();
	private List<List<ElementState>> playerAbilitiesCheckpoints = new List<List<ElementState>>();

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");

        // Undo system
        customSignals.Connect(CustomSignals.SignalName.RequestCheckpoint, new Callable(this, MethodName.RequestCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.AddCheckpoint, new Callable(this, MethodName.AddLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.UndoCheckpoint, new Callable(this, MethodName.UndoLocalCheckpoint));
        customSignals.Connect(CustomSignals.SignalName.ReplaceTopCheckpoint, new Callable(this, MethodName.ReplaceTopLocalCheckpoint));

        customSignals.Connect(CustomSignals.SignalName.SetPlayerPosition, new Callable(this, MethodName.SetPosition));
        //customSignals.Connect(CustomSignals.SignalName.GamePaused, new Callable(this, MethodName.PauseAbilityTimer));
        gameState = GetNode<GameState>("/root/GameState");
        levelTransitions = GetNode<CanvasLayer>("/root/Transitions") as LevelTransitions;
        audioManager = GetNode<Node>("/root/AudioManager") as AudioManager;

		particlesActive = GetNode<SettingsManager>("/root/SettingsManager").LightParticlesActive;
		if (!particlesActive) particles.QueueFree();
		wallslideSlowdownActive = GetNode<SettingsManager>("/root/SettingsManager").SpeedrunTimerVisible;

        gravity = defaultGravity;
		shaderScript = animatedSprite as PlayerShaderEffects;

		if (gameState.IsAbilitySalvagingUnlocked)
		{
			AbilityList = new List<ElementState>(gameState.SalvagedAbilities);
            customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityListUpdated, GameUtils.ElementListToIntArray(AbilityList));
        }
		else AbilityList = new List<ElementState>();

        BaseAbility = ElementState.normal;

		if (setPlayerRespawnPosition == true) // Respawning after dying or loading in a Hub (multiple checkpoints)
        {
            setPlayerRespawnPosition = false;
			if (gameState.PlayerHubRespawnPosition != Vector2.Zero)
				SetPosition(gameState.PlayerHubRespawnPosition);
        }

		//RequestCheckpointAfterTime(inputBufferTime);
		RequestCheckpoint();
    }

    public override void _PhysicsProcess(double delta)
	{
		// Get input
		direction = InputManager.GetLeftRightGameplayDirection();
		restartPressed = InputManager.RestartPressed();
        jumpPressed = InputManager.JumpPressed();
        jumpReleased = InputManager.JumpReleased();
        abilityPressed = InputManager.AbilityPressed();
		undoPressed = InputManager.UndoPressed();

        isGrounded = IsOnFloor();
		isClinging = IsOnWallOnly() && Velocity.Y > 0.001f && ((!isFacingRight && direction < 0.0f) || (isFacingRight && direction > 0.0f));

        if (!IsFrozen)
		{
            // Gravity
            if (!isGrounded)
                Velocity += new Vector2(0.0f, gravity * (float)delta);

            Jump(jumpPressed, jumpReleased, delta);
            Ability(abilityPressed, delta);
            Movement(direction);

            AttemptVerticalCornerCorrection(verticalCornerCorrection, (float)delta);
            MoveAndSlide();
            CheckForCollision(delta);
            TryAddCheckpoint(delta);
            //if (checkpointTimer > 0.0f) checkpointTimer -= (float)delta; // So cp cannot be requested multiple times in quick succession
        }
        UpdateAnimation(direction);

        if (!gameState.IsLevelTransitionPlaying)
        {
            if (restartPressed) RestartLevel();
            if (undoPressed) UndoCheckpoint();
        }

        // Pressed interact button
        if (InputManager.UpInteractPressed() && !IsFrozen) customSignals.EmitSignal(CustomSignals.SignalName.PlayerInteracted);

        // Progressed dialog
        if (gameState.CanProgressDialog)
        {
            if (InputManager.UpInteractPressed() || InputManager.JumpPressed())
                customSignals.EmitSignal(CustomSignals.SignalName.ProgressDialog);
        }

        //GD.Print(CanAddCheckpoint());
        //GD.Print(isOnMovingEntity);
    }

	// MOVEMENT ==================================================================================================================

	private void Movement(float direction)
	{
		//GD.Print(Velocity.Y);

		if (isUsingAbility) return;
		
		// left right movement
		if (jumpMovePreventionTimer <= 0.0f)
			Velocity = new Vector2(direction * speed, Velocity.Y);

		// Falling speed cap
		if (Velocity.Y >= maxFallingSpeed)
			Velocity = new Vector2(Velocity.X, maxFallingSpeed);

		// resistance when clinging to a wall
		if (isClinging) //  && wallslideSlowdownActive // taken away because its kinda cringe
            Velocity = new Vector2(Velocity.X, Velocity.Y * clingDrag);
	}


	private void Jump(bool jumpPressed, bool jumpReleased, double delta)
	{
		if (jumpMovePreventionTimer > 0.0f)
		{
			//Velocity = new Vector2(Velocity.X, 0.0f); // Fix of a very obscure earth ability tech
			jumpMovePreventionTimer -= (float)delta;
		}
        if (isUsingAbility || jumpMovePreventionTimer > 0.0f) return;

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
            coyoteTimeCounter = 0f;
            jumpBufferTimeCounter = 0f;
			footstepTimer = -0.1f; // random negative number so player makes sound falling on the ground
			isGrounded = false;
		}

		if (canJumpCancel && Velocity.Y < 0.0f && jumpReleased) // cancel jump
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y * jumpCancelFraction);
			coyoteTimeCounter = 0f;
            jumpBufferTimeCounter = 0f;
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
				gameState.NoAbilityUses++;

				if (AbilityList.Count == 0)
				{
					canUseBaseAbility = false;
					currentAbility = BaseAbility;
				}
				else
				{
					// REMOVE TOP ABILITY AND INVOKE ABILITY USED EVENT
					currentAbility = AbilityList[^1];
					GameUtils.ListRemoveLastElement(AbilityList);
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

				AttemptHorizontalCornerCorrection(windDashCornerCorrection, (float)delta);
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
					RequestCheckpointAfterTime(inputBufferTime);
                    StopAbility();
                    SparkleAbilityDust(ElementState.earth, 0.1f);
                    abilityBufferTimeCounter = -0.1f; // Preventing overlapping abilities
					isGrounded = false;
					Velocity = new Vector2(Velocity.X, earthJumpPower);
					audioManager.earthAbilityEnd.Play();
				}
				else Velocity = new Vector2(0f, dashPower);
			}
			else if (currentAbility == ElementState.love)
			{
                Velocity = new Vector2(0f, 0f);
            }
		}

		else if (isGrounded) canUseBaseAbility = true;
	}

	async private void ExecuteAbilityAfterSeconds(float t)
	{
        await ToSignal(GetTree().CreateTimer(t, processInPhysics: true), "timeout");

		if (isDead) // In case you die in the short period between starting and executing ability
		{
			StopAbility();
			return;
		}

        isExecutingAbility = true;
		StartAbilityDust(currentAbility);
		if (currentAbility != ElementState.love) shaderScript.ActivateTrail(currentAbility);

		if (currentAbility == ElementState.air)
		{
			audioManager.airAbility.Play();

			abilityTimer.Start(dashTime);
			await ToSignal(abilityTimer, "timeout");
            abilityTimer.Stop();

            StopAbility();

            RequestCheckpointAfterTime(abilityFreezeTime); // Horizontal ability bugfix
        }
		else if (currentAbility == ElementState.water)
		{
			audioManager.waterAbility.Play();

            abilityTimer.Start(dashTime / 2f);
            await ToSignal(abilityTimer, "timeout");
			abilityTimer.Stop();

            jumpMovePreventionTimer = -0.1f; // So your momentum doesnt get lost after teleporting
            StopAbility();

			RequestCheckpoint();
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
        else if (currentAbility == ElementState.love)
        {
			audioManager.loveAbility.Play();
            shaderScript.SpawnHeart();
            SparkleAbilityDust(ElementState.love, 0.1f);
            abilityBufferTimeCounter = -0.1f; // So it doesn't trigger twice

            abilityTimer.Start(dashTime);
            await ToSignal(abilityTimer, "timeout");
            abilityTimer.Stop();

            StopAbility();

            RequestCheckpoint();
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
		instance.AddToGroup("Fireball"); // For checking how many there are

		RequestCheckpoint();
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

	private void SetDirection(float direction)
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

    private void UpdateAnimation(float direction)
	{
		if (gameState.IsLevelTransitionPlaying && !isDead) animatedSprite.Play("Idle"); // Level transition animation
		if (gameState.IsDialogActive) animatedSprite.Play("Idle"); // dialog animation
		if (gameState.WatchtowerActive) animatedSprite.Play("Idle"); // watchtower animation
        if (IsFrozen) return;

		float animationSpeed = 1.0f;

		if (!isExecutingAbility && jumpMovePreventionTimer <= 0.0f) // you can change direction after you started ability before it executed for input leniency
		{
			SetDirection(direction);
		}

		// animations
		if (currentAbility == ElementState.air)
			currentAnimation = "Dash";
        else if (currentAbility == ElementState.earth)
            currentAnimation = "Dive";
        else if (currentAbility == ElementState.water)
			currentAnimation = "Jump";
		else if (currentAbility == ElementState.fire)
			currentAnimation = "Fireball";
        else if (currentAbility == ElementState.love)
		{
			currentAnimation = "Love";
			if (isGrounded) currentAnimation = "LoveGrounded";
        } 
        else if (isGrounded)
		{
			if (direction == 0.0f || IsOnWall() || jumpMovePreventionTimer > 0.0f)
				currentAnimation = "Idle";
			else
			{
				currentAnimation = "Run";
				animationSpeed = Math.Abs(direction);
			}
		}
		else
		{
			if (Velocity.Y < -1.0f) currentAnimation = "Jump";
			else if (isClinging) currentAnimation = "Cling";
			else if (Velocity.Y > 1.0f) currentAnimation = "Fall";
			else currentAnimation = "Idle";
		}

		animatedSprite.Play(currentAnimation, customSpeed:animationSpeed);
	}


    // DYING ======================================================================================================================

    private void _OnArea2dBodyEntered(Node2D body) // This is a small area in the player, if a body enters it the player got crushed
	{
		if (body is not Player && body is not Fireball && body is not TileMap && !SqueezeDeathDisabled)
			Kill(crushed:true);
	}

	private void RestartLevel()
	{
		gameState.NoRestarts++;

        if (gameState.IsHubLoaded() && gameState.PlayerHubRespawnPosition != Vector2.Zero) // Dying in Hub (multiple checkpoints)
            setPlayerRespawnPosition = true;

        isDead = true;
		if (animatedSprite.Animation != "Die" && !isGrounded) animatedSprite.Play("Die", customSpeed:1.8f);
        levelTransitions.StartLevelReloadTransition();
    }

    private void Kill(bool crushed = false)
    {
		gameState.NoDeaths++;

        if (!crushed) RequestCheckpoint(); // So we don't loose too much progress when undoing after death (unless crushed cuz it glitches)
        StopAbility();
        isDead = true;
        abilityBufferTimeCounter = -0.1f;
        animatedSprite.Play("Die"); // death animation
        currentAnimation = "Die";
        audioManager.death.Play();
        customSignals.EmitSignal(CustomSignals.SignalName.PlayerDied);

        //await ToSignal(GetTree().CreateTimer(t, processInPhysics: true), "timeout");
    }

    // COLLISIONS

    private void CheckForCollision(double delta)
	{
		isOnMovingEntity = false;

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            Node collider = collision.GetCollider() as Node;

            float angle = collision.GetAngle();
			bool isOnTopOfCollision = angle <= 0.001f && angle >= -0.001f;

            if (collider.IsInGroup("Danger"))
			{
				Kill();
				break;
			}

			if (collider.IsInGroup("Gate"))
			{
				Gate gate = collider.GetParent().GetParent() as Gate; // Accessing the root node of the gate (where the script is)
				if (gate.IsMovingUp) isOnMovingEntity = true;
            }

			// Making proper sound when walking on something
            if (collider.IsInGroup("PlayerCollider"))
            {
                if (!isOnTopOfCollision) continue; // Check if is touching floor

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

	private void AttemptHorizontalCornerCorrection(int allowedPixelsOff, float delta)
	{
		if (!TestMove(GlobalTransform, new Vector2(Velocity.X * delta, 0f))) return;

		for (int i = 1; i <= allowedPixelsOff; i++)
		{ 
			foreach (int j in new int[]{ -1, 1})
			{
				if (!TestMove(GlobalTransform.Translated(new Vector2(0f, i*j)), new Vector2(Velocity.X * delta, 0f)))
				{
					Translate(new Vector2(0f, i * j));
					return;
				}
			}
		}
	}

    private void AttemptVerticalCornerCorrection(int allowedPixelsOff, float delta)
    {
        if (Velocity.Y >= 0f || !TestMove(GlobalTransform, new Vector2(0f, Velocity.Y * delta))) return;

        for (int i = 1; i <= allowedPixelsOff; i++)
        {
            foreach (int j in new int[] { -1, 1 })
            {
                if (!TestMove(GlobalTransform.Translated(new Vector2(i * j, 0f)), new Vector2(0f, Velocity.Y * delta)))
                {
                    Translate(new Vector2(i * j, 0f));
                    return;
                }
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

	public void SetPosition(Vector2 position, bool fireTeleport = false, float fireballDirection = 0f)
	{
        hitbox.Disabled = true; // bugfix
		
        Velocity = new Vector2(0f, 0f);
        jumpMovePreventionTimer = jumpPreventionTime;
        coyoteTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;
        canJumpCancel = false;
        isGrounded = false;

        if (fireTeleport)
        {
            shaderScript.SpawnFireTeleportResidue();
            RequestCheckpointAfterTime(inputBufferTime);

			//SetDirection(fireballDirection);
            //position = new Vector2(position.X - (fireballDirection * 2), position.Y);
			position = new Vector2(position.X, position.Y - 1f);
        }

        GlobalPosition = new Vector2(position.X, (float)Math.Floor(position.Y));

        ReformHitboxEndFrame();
    }

    private async void ReformHitboxEndFrame()
    {
        await ToSignal(GetTree(), "process_frame");
        hitbox.Disabled = false;
    }

    // UNDO SYSTEM =============================================================================================================

    private void RequestCheckpoint()
	{
		if (isUndoing) return;
        checkpointRequested = true;
		//SetCheckpointIndicatorPosition();
    }

	private async void RequestCheckpointAfterTime(float t)
	{
        await ToSignal(GetTree().CreateTimer(t, processInPhysics: true), "timeout");
		RequestCheckpoint();
    }

	private void SetCheckpointIndicatorPosition(bool undone = false)
	{
        if (playerPositionCheckpoints.Count == 0) return;

        checkpointIndicator.Show();

        if (checkpointRequested && !undone)
		{
            checkpointIndicator.GlobalPosition = playerPositionCheckpoints[^1] - new Vector2(0, GameUtils.gameUnitSize);
        }
		else if (playerPositionCheckpoints.Count > 1)
		{
			checkpointIndicator.GlobalPosition = playerPositionCheckpoints[^2] - new Vector2(0, GameUtils.gameUnitSize);
        }
		else
		{
            checkpointIndicator.GlobalPosition = playerPositionCheckpoints[^1] - new Vector2(0, GameUtils.gameUnitSize);
        }
    }

    // No checkpoint when there is a fireball lingering, you need to be grounded and be able to jump, there also shouldn't be a moving gate and you need to be able to die from being crushed
    private bool CanAddCheckpoint() => isGrounded && !isUsingAbility && jumpMovePreventionTimer <= 0.0f && GetTree().GetNodesInGroup("Fireball").Count == 0 && !isOnMovingEntity && !IsOnWall() && !SqueezeDeathDisabled;

    private void TryAddCheckpoint(double delta)
	{
		checkpointTimer -= (float)delta;

        if (!checkpointRequested) return;

        if (CanAddCheckpoint())
		{
            checkpointRequested = false;

            if (playerPositionCheckpoints.Count == 0 || checkpointTimer <= 0.0f)
			{
				//GD.Print("CP Added");
                customSignals.EmitSignal(CustomSignals.SignalName.AddCheckpoint);
            }
			else
			{
                //GD.Print("CP Replaced");
                customSignals.EmitSignal(CustomSignals.SignalName.ReplaceTopCheckpoint);
            }

            checkpointTimer = inputBufferTime;
        }
    }

	private void UndoCheckpoint()
	{
		if (isUsingAbility || gameState.IsDialogActive || gameState.WatchtowerActive) return;
		if (playerPositionCheckpoints.Count == 0) return;

		gameState.NoUndos++;

		isUndoing = true;
		hitbox.Disabled = true; // bugfix

        customSignals.EmitSignal(CustomSignals.SignalName.UndoCheckpoint, checkpointRequested);
        checkpointRequested = false;
    }

    public void AddLocalCheckpoint()
    {
        playerPositionCheckpoints.Add(GlobalPosition);
        playerAbilitiesCheckpoints.Add(new List<ElementState>(AbilityList));
        //SetCheckpointIndicatorPosition();
    }

    public void UndoLocalCheckpoint(bool nextCpRequested)
    {
        if (isDead) isDead = false;

        if (!checkpointRequested && playerPositionCheckpoints.Count > 1)
        {
            GameUtils.ListRemoveLastElement(playerPositionCheckpoints);
            GameUtils.ListRemoveLastElement(playerAbilitiesCheckpoints);
        }

        propertyAnimations.Play("FadeOut");

        SetUndoPosition(playerPositionCheckpoints[^1]);
        AbilityList = new List<ElementState>(playerAbilitiesCheckpoints[^1]);

        customSignals.EmitSignal(CustomSignals.SignalName.PlayerAbilityListUpdated, GameUtils.ElementListToIntArray(AbilityList));
        
        //SetCheckpointIndicatorPosition(true);
    }

    public void ReplaceTopLocalCheckpoint()
    {
        GameUtils.ListRemoveLastElement(playerPositionCheckpoints);
        GameUtils.ListRemoveLastElement(playerAbilitiesCheckpoints);
        AddLocalCheckpoint();
    }

    public void SetUndoPosition(Vector2 position)
    {
        StopAbility();
        SetUndoPositionAfterTime(inputBufferTime, position);
    }

    private async void SetUndoPositionAfterTime(float t, Vector2 position)
    {
        await ToSignal(GetTree().CreateTimer(t, processInPhysics: true), "timeout");
        SetPosition(position);
        customSignals.EmitSignal(CustomSignals.SignalName.SetCameraPosition, GlobalPosition);
        isUndoing = false;
		propertyAnimations.Play("FadeIn");
    }
}
