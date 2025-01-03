using System.Numerics;
using Astralis.Core.Numerics;
using Astralis.Core.Types;
using Astralis.Game.Client.Components.Entities;
using Astralis.Game.Client.Core.Visuals;
using Astralis.Game.Client.Ecs.GameObjects.Base;
using Astralis.Game.Client.Interfaces.Services;
using ImGuiNET;
using Silk.NET.Input;

namespace Astralis.Game.Client.Ecs.GameObjects;

public class PlayerGameObject : BaseGameObject, IDebuggableComponent
{
    public string Name => "Player";

    public string Category => "Player";

    public Vector3 Position
    {
        get => _camera.Position;
        set => _camera.Position = value;
    }

    public Vector3 Direction
    {
        get => _camera.Front;
        set => _camera.Front = value;
    }

    public float MoveSpeed { get; set; } = 5.0f;
    public float SprintSpeed { get; set; } = 50.0f;

    private readonly Camera _camera;
    private readonly IKeyboard _keyboard;
    private readonly IMouse _mouse;

    private readonly IOpenGlContext _openGlContext;

    private Vector3 _lastPosition = Vector3.Zero;

    public Vector3Direction PlayerDirection { get; set; } = Vector3Direction.None;


    public PlayerGameObject(IOpenGlContext openGlContext, Camera camera, Vector3 position = default)
    {
        _openGlContext = openGlContext;
        _camera = camera;
        AstralisGameInstances.Camera = _camera;
        _keyboard = openGlContext.PrimaryKeyboard;
        _mouse = openGlContext.PrimaryMouse;
        Position = position == default ? Vector3.Zero : position;

        SetupVariables();
    }

    public override void Update(double deltaTime)
    {
        Move(deltaTime);

        base.Update(deltaTime);
    }

    private void SetupVariables()
    {
        AstralisGameInstances.VariablesService().AddVariableBuilder("player.position", () => Position);
        AstralisGameInstances.VariablesService().AddVariableBuilder("player.direction", () => Direction);
        AstralisGameInstances.VariablesService().AddVariableBuilder("camera.zoom", () => _camera.Zoom);
        AstralisGameInstances.VariablesService().AddVariableBuilder("camera.yaw", () => _camera.Yaw);
        AstralisGameInstances.VariablesService().AddVariableBuilder("camera.pitch", () => _camera.Pitch);
    }


    private void Move(double deltaTime)
    {
        var speed = MoveSpeed * (float)deltaTime;


        if (_keyboard.IsKeyPressed(Key.ShiftLeft))
        {
            speed = SprintSpeed * (float)deltaTime;
            _camera.Zoom = MathF.Min(65f, (float)(_camera.Zoom + (10f * deltaTime)));
        }
        else
        {
            _camera.Zoom = MathF.Max(60f, (float)(_camera.Zoom - (100f * deltaTime)));
        }

        if (_keyboard.IsKeyPressed(Key.W))
        {
            Position += _camera.Front * speed;
        }

        if (_keyboard.IsKeyPressed(Key.S))
        {
            Position -= _camera.Front * speed;
        }

        if (_keyboard.IsKeyPressed(Key.A))
        {
            Position -= _camera.Right * speed;
        }

        if (_keyboard.IsKeyPressed(Key.D))
        {
            Position += _camera.Right * speed;
        }

        if (_keyboard.IsKeyPressed(Key.Space))
        {
            Position += _camera.WorldUp * speed;
        }

        if (_keyboard.IsKeyPressed(Key.ControlLeft))
        {
            Position -= _camera.WorldUp * speed;
        }


        PlayerDirection = MathHelper.GetDirection(_lastPosition, Position);
        _lastPosition = Position;
    }


    public void DebugRender()
    {
        ImGui.Text("Position: " + Position);
        ImGui.Text("Direction: " + PlayerDirection);
        ImGui.Text("Zoom: " + _camera.Zoom);
        ImGui.Text("Yaw: " + _camera.Yaw);
        ImGui.Text("Pitch: " + _camera.Pitch);
    }
}
