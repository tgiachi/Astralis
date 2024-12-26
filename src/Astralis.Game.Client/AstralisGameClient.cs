using System.Drawing;
using Microsoft.Extensions.Hosting;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Astralis.Game.Client;

public class AstralisGameClient : IHostedService
{
    public static GL Gl { get; private set; }
    private readonly IWindow _window;


    private IInputContext _inputContext;


    public AstralisGameClient(IWindow window)
    {
        _window = window;
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
    }

    private void OnRender(double deltaTime)
    {
        Gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private void OnUpdate(double deltaTime)
    {
    }

    private void OnLoad()
    {
        _inputContext = _window.CreateInput();
        Gl = _window.CreateOpenGL();

        Gl.ClearColor(Color.CornflowerBlue);
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
