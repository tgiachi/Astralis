using System.Drawing;
using Microsoft.Extensions.Hosting;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Astralis.Game.Client;

public class AstralisGameClient : IHostedService
{
    private readonly WindowOptions _windowOptions;
    private readonly IWindow _window;


    private IInputContext _inputContext;

    private GL _gl;

    public AstralisGameClient()
    {
        _windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Astralis Game Client v0.0.1"
        };

        _window = Window.Create(_windowOptions);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
    }

    private void OnRender(double deltaTime)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private void OnUpdate(double deltaTime)
    {
    }

    private void OnLoad()
    {
        _inputContext = _window.CreateInput();
        _gl = GL.GetApi(_window);
        _gl.ClearColor(Color.CornflowerBlue);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => _window.Run(), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _window.Close();
        return Task.CompletedTask;
    }
}
