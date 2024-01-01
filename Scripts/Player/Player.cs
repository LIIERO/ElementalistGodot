using Godot;
using System;
using static GlobalTypes;

public partial class Player : CharacterBody2D
{
    // Child nodes
    [Export] private AnimatedSprite2D animatedSprite;
    [Export] private ShapeCast2D wallcheck;


    // Parameters
    public const float speed = 110.0f;
    public const float jumpVelocity = -220.0f;
    public const float maxFallingSpeed = 250f;

    public const float earthJumpPower = 17.0f;
    public const float dashPower = 15.0f;
    public const float dashTime = 0.2f;
    public const float waterJumpMomentumPreservation = 0.6f;
    public const float clingDrag = 0.7f;
    public const float coyoteTime = 0.1f;
    public const float inputBufferTime = 0.1f;
    public const float abilityFreezeTime = 0.05f;
    public const float defaultGravityScale = 4.0f;
    public const float jumpCancelFraction = 0.2f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    // Player state (move to a different class?)
    ElementState currentAbility = ElementState.normal;
    bool isUsingAbility = false;
    bool isExecutingAbility = false;
    bool isFacingRight = true;
    //(bool left, bool right) huggingWall = (false, false); // useless I think
    float coyoteTimeCounter; float jumpBufferTimeCounter; float abilityBufferTimeCounter;
    

    public override void _Ready()
    {

    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        // Handle Jump
        bool jumpPressed = Input.IsActionJustPressed("inputJump");
        bool jumpReleased = Input.IsActionJustReleased("inputJump");
        velocity = Jump(jumpPressed, jumpReleased, velocity, delta);

		// Get the input direction and handle the movement/deceleration
		// As good practice, you should replace UI actions with custom gameplay actions
		Vector2 direction = Input.GetVector("inputLeft", "inputRight", "inputUp", "inputDown");
		velocity = Movement(velocity, direction);

        UpdateAnimation(direction);


        Velocity = velocity;
		MoveAndSlide();

	}


    private Vector2 Movement(Vector2 velocity, Vector2 direction)
    {
        if (isUsingAbility) return velocity;
        
        // left right movement
        velocity.X = direction.X * speed;
        if (velocity.Y >= maxFallingSpeed)
            velocity.Y = maxFallingSpeed;

        // resistance when clinging to a wall
        if (IsOnWallOnly() && velocity.Y > 0f)
            velocity.Y *= clingDrag;
        
        return velocity;
    }


    private Vector2 Jump(bool jumpPressed, bool jumpReleased, Vector2 velocity, double delta)
    {
        if (isUsingAbility) return velocity;

        if (IsOnFloor())
            coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= (float)delta;

        if (jumpPressed)
            jumpBufferTimeCounter = inputBufferTime;
        else jumpBufferTimeCounter -= (float)delta;

        if (jumpBufferTimeCounter > 0.0f && coyoteTimeCounter > 0.0f) // jump
        {
            // jump_sound.Play();
            // CreateJumpDust();
            velocity.Y = jumpVelocity;
            jumpBufferTimeCounter = 0f;
        }

        if (velocity.Y < 0.0f && jumpReleased) // cancel jump
        {
            velocity.Y *= jumpCancelFraction;
            coyoteTimeCounter = 0f;
        }
        
        return velocity;
    }


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
        else if (IsOnFloor())
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

        if (GlobalData.PlayingCutscene)
        {
            animation = "Idle";
            Velocity = new Vector2(0f, 0f);
        }

        animatedSprite.Play(animation);
    }





    // PUBLIC FUNCTIONS ==================================================================================================================

    

}
