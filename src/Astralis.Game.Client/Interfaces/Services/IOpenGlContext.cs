using System.Numerics;
using Astralis.Core.Numerics;
using Astralis.Game.Client.Data;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Astralis.Game.Client.Interfaces.Services;

public interface IOpenGlContext
{
    AstralisGameConfig Config { get; }
    event Action<double> OnUpdateEvent;
    event Action<double, GL> OnRenderEvent;
    event Action<GL> OnStartEvent;
    IWindow Window { get; }
    IInputContext Input { get; }
    GL Gl { get; }
    IKeyboard PrimaryKeyboard { get; }
    IMouse PrimaryMouse { get; }
    void Run();
    void Stop();
    unsafe void SetCursorMode(CursorModeValue cursorMode);
    bool CursorIsNotAvailable();
    unsafe CursorModeValue GetCursorMode();

    double Fps { get; }
    Vector2Int GetWindowSize();
}
