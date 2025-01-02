using System.Numerics;
using Astralis.Core.Numerics;
using Astralis.Game.Client.Core.Collision;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Astralis.Game.Client.Core.Visuals;

public class Camera
{
    public Vector3 Position { get; set; }

    public Vector3 UiPosition = new Vector3(0, 0, 3);
    public Vector3 Front { get; set; }
    public Vector3 Up { get; private set; }
    public readonly Vector3 WorldUp = Vector3.UnitY;
    public Vector3 Right { get; private set; }
    public float AspectRatio { get; set; }
    public float Yaw { get; set; } = -90f;
    public float Pitch { get; set; }

    public float NearDistance { get; set; } = 0.1f;
    public float FarDistance { get; set; } = 1000f;

    public float Zoom { get; set; } = 60f;
    private Vector2 _lastMousePosition;
    private readonly bool _isZoomActive = false;
    private readonly IMouse? _mouse;
    public readonly bool frustrumUpdate = true;

    private readonly Frustum _frustum;

    public Camera(IWindow? window = null, IMouse? mouse = null)
    {
        _mouse = mouse;
        Setup(Vector3.Zero, Vector3.UnitZ * 1, WorldUp, 800f / 600f);
        _frustum = new Frustum(this);

        if (window is null)
        {
            return;
        }

        Vector2D<int> size = window.GetFullSize();
        AspectRatio = size.X / (float)size.Y;
        window.FramebufferResize += FrameBufferResize;
        mouse.Cursor.CursorMode = CursorMode.Normal;
        mouse.MouseMove += OnMouseMove;
    }

    public void SetZoomActive(bool active)
    {
        if (_isZoomActive == active)
        {
            return;
        }

        if (active)
        {
            _mouse!.Scroll += OnMouseWheel;
        }
        else
        {
            _mouse!.Scroll -= OnMouseWheel;
        }
    }

    private void Setup(Vector3 position, Vector3 front, Vector3 up, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
        Front = front;
        Up = up;
        Right = Vector3.Normalize(Vector3.Cross(Front, up));
    }

    public Frustum GetFrustrum()
    {
        UpdateFrustum();
        return _frustum;
    }

    public void UpdateFrustum()
    {
        if (frustrumUpdate)
        {
            _frustum.Update(this);
        }
    }


    public void ModifyZoom(float zoomAmount)
    {
        //We don't want to be able to zoom in too close or too far away so clamp to these values
        Zoom = Math.Clamp(Zoom - zoomAmount, 1.0f, 45f);
    }

    public void ModifyDirection(float xOffset, float yOffset)
    {
        Yaw += xOffset;
        Pitch -= yOffset;

        //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
        Pitch = Math.Clamp(Pitch, -89f, 89f);

        var cameraDirection = Vector3.Zero;
        cameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw) * MathF.Cos(MathHelper.DegreesToRadians(Pitch)));
        cameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        cameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

        Front = Vector3.Normalize(cameraDirection);

        Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }

    public Matrix4x4 GetViewMatrix() => Matrix4x4.CreateLookAt(Position, Position + Front, Up);

    public Matrix4x4 GetUiViewMatrix() => Matrix4x4.CreateLookAt(UiPosition, UiPosition + new Vector3(0, 0, -1), WorldUp);

    public Matrix4x4 GetProjectionMatrix() => Matrix4x4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(Zoom),
        AspectRatio,
        NearDistance,
        FarDistance
    );

    public Matrix4x4 GetUiProjectionMatrix() => Matrix4x4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(60),
        AspectRatio,
        NearDistance,
        FarDistance
    );

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        var lookSensitivity = 0.1f;
        if (_lastMousePosition == default)
        {
            _lastMousePosition = position;
        }
        else
        {
            var xOffset = (position.X - _lastMousePosition.X) * lookSensitivity;
            var yOffset = (position.Y - _lastMousePosition.Y) * lookSensitivity;
            _lastMousePosition = position;

            ModifyDirection(xOffset, yOffset);
        }
    }

    private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
    {
        ModifyZoom(scrollWheel.Y);
    }

    private void FrameBufferResize(Vector2D<int> size)
    {
        AspectRatio = (float)size.X / (float)size.Y;
    }

    /*public void DisplayFrustrum()
    {
        float halfVSide = FarDistance * MathF.Tan(MathHelper.DegreesToRadians(Zoom) / 2);
        float halfHSide = halfVSide * AspectRatio;
        new Line(
            position,
            position + (
                Front * farDistance) +
            (up * halfVSide) +
            (Right * halfHSide)
        );
        new Line(
            position,
            position + (
                Front * farDistance) +
            (up * halfVSide) +
            (-Right * halfHSide)
        );

        new Line(
            position,
            position + (
                Front * farDistance) +
            (-up * halfVSide) +
            (Right * halfHSide)
        );
        new Line(
            position,
            position + (
                Front * farDistance) +
            (-up * halfVSide) +
            (-Right * halfHSide)
        );


        new Line(
            position + (
                Front * farDistance) +
            (up * halfVSide) +
            (Right * halfHSide),
            position + (
                Front * farDistance) +
            (up * halfVSide) +
            -(Right * halfHSide)
        );
        new Line(
            position + (
                Front * farDistance) +
            -(up * halfVSide) +
            (Right * halfHSide),
            position + (
                Front * farDistance) +
            -(up * halfVSide) +
            -(Right * halfHSide)
        );
    }*/
}
