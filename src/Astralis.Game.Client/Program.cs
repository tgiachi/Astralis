using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Astralis.Game.Client;

class Program
{
    private static IWindow _window;
    private static GL _gl;

    static void Main(string[] args)
    {
        WindowOptions options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "My first Silk.NET application!",
            //API = GraphicsAPI.DefaultVulkan
        };

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Render += Render;
        _window.Update += Update;
        _window.Run();
    }

    private static void OnLoad()
    {
        _gl = _window.CreateOpenGL();
    }

    private static void Update(double obj)
    {
    }

    private static void Render(double obj)
    {
        _window.Title = $"FPS: {_window.FramesPerSecond}";

        _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        _gl.ClearColor(Color.CornflowerBlue);

        // draw
        //  _window.SwapBuffers();
    }
}
