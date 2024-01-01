using Godot;
using System;

public partial class Camera : Camera2D
{
    public static bool FreeViewModeEnabled { get; set; } = false; // TODO: move to a freeviewcontroller
    [Export] private Node2D PlayerNode { get; set; }

    Vector2 initialPosition;

    // How far in each direction can camera go in grid units
    [Export] private float leftLimit = -99f;
    [Export] private float upLimit = -99f;
    [Export] private float rightLimit = 99f;
    [Export] private float downLimit = 99f;

    private const float freeViewSpeed = 200.0f;

    Vector2 desiredPosition;

    public override void _Ready()
	{
        initialPosition = Position;

        leftLimit = (leftLimit * GlobalData.gameUnitSize) + initialPosition.X;
        upLimit = (upLimit * GlobalData.gameUnitSize) + initialPosition.Y;
        rightLimit = (rightLimit * GlobalData.gameUnitSize) + initialPosition.X;
        downLimit = (downLimit * GlobalData.gameUnitSize) + initialPosition.Y;

        Position = ApplyCameraBounds(Position);
    }

	public override void _Process(double delta)
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

        Position = desiredPosition;

        //fwc.CheckIfCameraTouchesBounds(desiredPosition, cameraBoundaries);

        //Vector3 smoothedPosition = Vector3.SmoothDamp(cameraObject.transform.position, desiredPosition, ref velocity, smoothCameraTime);
        //cameraObject.transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, cameraObject.transform.position.z); // Camera following player

    }

    Vector2 ApplyCameraBounds(Vector2 position)
    {
        if (position.X < leftLimit) position.X = leftLimit;
        else if (position.X > rightLimit) position.X = rightLimit;
        
        if (position.Y < upLimit) position.Y = upLimit;
        else if (position.Y > downLimit) position.Y = downLimit;

        return position;
    }
}
