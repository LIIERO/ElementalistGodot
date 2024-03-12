using Godot;
using System;

public partial class Camera : Camera2D
{
    // Singletons
    private GameState gameState;
    private CustomSignals customSignals;

    public static bool FreeViewModeEnabled { get; set; } = false; // TODO: move to a freeviewcontroller
    [Export] private Node2D PlayerNode { get; set; }

    Vector2 initialPosition;

    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    const float smoothTime = 0.2f;

    // How far in each direction can camera go in grid units
    [Export] private float leftLimit = -99f;
    [Export] private float upLimit = -99f;
    [Export] private float rightLimit = 99f;
    [Export] private float downLimit = 99f;
    private float baseLeftLimit, baseRightLimit, baseDownLimit, baseUpLimit;

    private const float freeViewSpeed = 200.0f;

    private Vector2 desiredPosition;

    private bool snap = false; // Should camera snap to player when transition is playing (it should at the start)

    public override void _Ready()
	{
        gameState = GetNode<GameState>("/root/GameState");
        if (gameState.IsLevelTransitionPlaying) snap = true;

        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.SetCameraPosition, new Callable(this, MethodName.SetPosition));
        customSignals.Connect(CustomSignals.SignalName.ShiftCameraXLimits, new Callable(this, MethodName.ShiftXLimits));

        initialPosition = Position;

        if (gameState.IsHubLoaded() && gameState.PlayerHubRespawnPosition != Vector2.Inf)
            Position = gameState.PlayerHubRespawnPosition;

        leftLimit = (leftLimit * GameUtils.gameUnitSize) + initialPosition.X;
        upLimit = (upLimit * GameUtils.gameUnitSize) + initialPosition.Y;
        rightLimit = (rightLimit * GameUtils.gameUnitSize) + initialPosition.X;
        downLimit = (downLimit * GameUtils.gameUnitSize) + initialPosition.Y;
        baseLeftLimit = leftLimit; baseRightLimit = rightLimit; baseDownLimit = downLimit; baseUpLimit = upLimit;

        Position = ApplyCameraLimits(Position);
    }

	public override void _Process(double delta)
	{
        ApplyPosition(delta);
    }

    private void ApplyPosition(double delta)
    {
        /*if (FreeViewModeEnabled)
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            float changeInPosition = freeViewSpeed * (float)delta;
            if (Input.GetKey(Global.kcUp)) desiredPosition += new Vector3(0, changeInPosition, 0);
            else if (Input.GetKey(Global.kcDown)) desiredPosition += new Vector3(0, -changeInPosition, 0);
            if (Input.GetKey(Global.kcRight)) desiredPosition += new Vector3(changeInPosition, 0, 0);
            else if (Input.GetKey(Global.kcLeft)) desiredPosition += new Vector3(-changeInPosition, 0, 0);
        }
        else desiredPosition = playerNode.Position;*/

        desiredPosition = PlayerNode.Position;
        desiredPosition = ApplyCameraLimits(desiredPosition);

        float smoothedX, smoothedY;
        smoothedX = GameUtils.SmoothDamp(Position.X, desiredPosition.X, ref velocityX, smoothTime, 200, (float)delta);
        smoothedY = GameUtils.SmoothDamp(Position.Y, desiredPosition.Y, ref velocityY, smoothTime, 200, (float)delta);

        if (snap) // Snap the camera during transition so it doesnt scroll towards you slowly
        {
            smoothedX = desiredPosition.X;
            smoothedY = desiredPosition.Y;
        }
        if (!gameState.IsLevelTransitionPlaying) snap = false;

        Position = new Vector2(smoothedX, smoothedY);

        //fwc.CheckIfCameraTouchesBounds(desiredPosition, cameraBoundaries);

        //Vector3 smoothedPosition = Vector3.SmoothDamp(cameraObject.transform.position, desiredPosition, ref velocity, smoothCameraTime);
        //cameraObject.transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, cameraObject.transform.position.z); // Camera following player
    }

    private Vector2 ApplyCameraLimits(Vector2 position)
    {
        if (position.X < leftLimit) position.X = leftLimit;
        else if (position.X > rightLimit) position.X = rightLimit;
        
        if (position.Y < upLimit) position.Y = upLimit;
        else if (position.Y > downLimit) position.Y = downLimit;

        return position;
    }

    public void SetPosition(Vector2 position)
    {
        GlobalPosition = ApplyCameraLimits(position);
    }

    public void ShiftXLimits(int left, int right)
    {
        float newLeftLimit = baseLeftLimit + left * GameUtils.gameUnitSize;
        if (Mathf.Abs(newLeftLimit) > Mathf.Abs(leftLimit)) leftLimit = newLeftLimit;

        float newRightLimit = baseRightLimit + right * GameUtils.gameUnitSize;
        if (Mathf.Abs(newRightLimit) > Mathf.Abs(rightLimit)) rightLimit = newRightLimit;
    }
}
