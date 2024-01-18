using Godot;
using System;

public partial class Camera : Camera2D
{
    private CustomSignals customSignals; // Singleton
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

    private const float freeViewSpeed = 200.0f;

    Vector2 desiredPosition;

    public override void _Ready()
	{
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.Connect(CustomSignals.SignalName.SetCameraPosition, new Callable(this, MethodName.SetPosition));
        initialPosition = Position;

        leftLimit = (leftLimit * GameUtils.gameUnitSize) + initialPosition.X;
        upLimit = (upLimit * GameUtils.gameUnitSize) + initialPosition.Y;
        rightLimit = (rightLimit * GameUtils.gameUnitSize) + initialPosition.X;
        downLimit = (downLimit * GameUtils.gameUnitSize) + initialPosition.Y;

        Position = ApplyCameraBounds(Position);
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
        desiredPosition = ApplyCameraBounds(desiredPosition);

        float smoothedX = GameUtils.SmoothDamp(Position.X, desiredPosition.X, ref velocityX, smoothTime, 200, (float)delta);
        float smoothedY = GameUtils.SmoothDamp(Position.Y, desiredPosition.Y, ref velocityY, smoothTime, 200, (float)delta);
        Position = new Vector2(smoothedX, smoothedY);

        //fwc.CheckIfCameraTouchesBounds(desiredPosition, cameraBoundaries);

        //Vector3 smoothedPosition = Vector3.SmoothDamp(cameraObject.transform.position, desiredPosition, ref velocity, smoothCameraTime);
        //cameraObject.transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, cameraObject.transform.position.z); // Camera following player
    }

    private Vector2 ApplyCameraBounds(Vector2 position)
    {
        if (position.X < leftLimit) position.X = leftLimit;
        else if (position.X > rightLimit) position.X = rightLimit;
        
        if (position.Y < upLimit) position.Y = upLimit;
        else if (position.Y > downLimit) position.Y = downLimit;

        return position;
    }

    public void SetPosition(Vector2 position)
    {
        GlobalPosition = ApplyCameraBounds(position);
    }
}
