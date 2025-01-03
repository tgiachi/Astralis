﻿using System.Runtime.InteropServices;
using System.Text;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Numerics;
using Astralis.Core.Server.Events.Engine;
using Astralis.Game.Client.Data;
using Astralis.Game.Client.Interfaces.Services;
using Serilog;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using Color = System.Drawing.Color;
using Glfw = Silk.NET.GLFW.Glfw;
using Monitor = Silk.NET.Windowing.Monitor;

namespace Astralis.Game.Client.Impl;

public class OpenGlContext
    : IOpenGlContext
{
    // instance fields
    public double Fps => _fps;

    public double MsPerFrame => 1000.0 / _fps;
    public AstralisGameConfig Config { get; }
    public event Action<double> OnUpdateEvent;
    public event Action<double, GL> OnRenderEvent;
    public event Action<GL> OnStartEvent;

    private readonly IEventBusService _eventBusService;

    private readonly ILogger _logger = Log.ForContext<OpenGlContext>();
    public IWindow Window { get; private set; }
    public IInputContext Input { get; private set; } = null!;
    public GL Gl { get; private set; } = null!;
    public IKeyboard PrimaryKeyboard { get; private set; } = null!;
    public IMouse PrimaryMouse { get; private set; } = null!;

    private double _elapsedTime = 0.0;
    private int _frameCount = 0;
    private double _fps = 0.0;


    private ImGuiController imGuiController = null!;

    public uint UboWorldHandle { get; private set; }
    public uint UboUiHandle { get; private set; }

    public static readonly Color DEFAULT_CLEAR_COLOR = Color.CornflowerBlue;
    public Color ClearColor = DEFAULT_CLEAR_COLOR;

    private Glfw glfw;

    private bool running = true;

    public OpenGlContext(AstralisGameConfig config, IEventBusService eventBusService)
    {
        Config = config;
        _eventBusService = eventBusService;
        //Create a window.
        var options = WindowOptions.Default;

        if (config.FullScreen)
        {
            IMonitor mainMonitor = Monitor.GetMainMonitor(null);
            if (mainMonitor.VideoMode.Resolution is null)
            {
                options.Size = config.WindowSize;
                options.WindowState = WindowState.Normal;
            }
            else
            {
                options.VideoMode = mainMonitor.VideoMode;
                options.Size = mainMonitor.VideoMode.Resolution.Value;
                options.WindowState = WindowState.Fullscreen;
            }
        }
        else
        {
            options.Size = config.WindowSize;
        }

        options.Title = options.Title;
        options.Samples = 4; //Anti-aliasing

        options.VSync = config.EnableVSync;
        options.API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 1));
        _logger.Information("View thread id: {ThreadId}", Environment.CurrentManagedThreadId);
        Window = Silk.NET.Windowing.Window.Create(options);

        //Assign events.
        Window.Load += OnLoad;
        Window.Render += OnRender;
        Window.Update += OnUpdate;
        Window.Closing += OnClose;
        Window.FramebufferResize += FrameBufferResize;

        glfw = Glfw.GetApi();
    }

    public void Run()
    {
        //Run the window.
        Window.Run();
    }

    public void Stop()
    {
        running = false;
    }

    private unsafe void OnLoad()
    {
        // view thread id

        //Set-up input context.
        Input = Window.CreateInput();
        Gl = Window.CreateOpenGL();
        Gl.GetInteger(GetPName.MajorVersion, out int major);
        Gl.GetInteger(GetPName.MinorVersion, out int minor);

        _logger.Information("OpenGL version {Major}.{Minor}", major, minor);

        byte* bytePtr = Gl.GetString(StringName.Renderer);
        List<byte> vendor = [];
        while (*bytePtr != 0)
        {
            vendor.Add(*bytePtr);
            bytePtr++;
        }

        _logger.Information("Vendor : {Vendor}", Encoding.ASCII.GetString(vendor.ToArray()));
        //  LoadIcon();

        // imGuiController = new ImGuiController(Gl, Window, Input, Fonts.DEFAULT_FONT_CONFIG, () => Fonts.LoadFonts());
        imGuiController = new ImGuiController(Gl, Window, Input);
        //   ImGuiPlus.SetupStyle();


        PrimaryKeyboard = Input.Keyboards.FirstOrDefault()!;
        PrimaryMouse = Input.Mice.FirstOrDefault()!;
        PrimaryKeyboard.KeyDown += KeyDown;

        //EnableFaceCulling();
        //EnableAntiAliasing();
        // EnableBlending();

        OnStartEvent?.Invoke(Gl);
        _eventBusService.Publish(new EngineStartedEvent());
        //        game.Start(Gl);
    }


    // private void LoadIcon()
    // {
    //     DecoderOptions options = new DecoderOptions();
    //     options.Configuration.PreferContiguousImageBuffers = true;
    //     using (var img = Image.Load<Rgba32>(options, Generated.FilePathConstants.Sprite.minecraftLogo_png))
    //     {
    //         img.DangerousTryGetSinglePixelMemory(out var imageSpan);
    //         var imageBytes = MemoryMarshal.AsBytes(imageSpan.Span).ToArray();
    //         RawImage[] iconsApp = new[] { new RawImage(img.Width, img.Height, imageBytes) };
    //         ReadOnlySpan<RawImage> span = new ReadOnlySpan<RawImage>(iconsApp);
    //         Window.SetWindowIcon(span);
    //     }
    // }


    private unsafe void InitUniformBuffers()
    {
        UboWorldHandle = Gl.GenBuffer();
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, UboWorldHandle);
        Gl.BufferData(BufferTargetARB.UniformBuffer, (nuint)(2 * sizeof(Matrix4X4<float>)), null, GLEnum.StaticDraw);
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        Gl.BindBufferRange(BufferTargetARB.UniformBuffer, 0, UboWorldHandle, 0, (nuint)(2 * sizeof(Matrix4X4<float>)));


        UboUiHandle = Gl.GenBuffer();
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, UboUiHandle);
        Gl.BufferData(BufferTargetARB.UniformBuffer, (nuint)(2 * sizeof(Matrix4X4<float>)), null, GLEnum.StaticDraw);
        Gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        Gl.BindBufferRange(BufferTargetARB.UniformBuffer, 1, UboUiHandle, 0, (nuint)(2 * sizeof(Matrix4X4<float>)));
    }

    private void EnableFaceCulling()
    {
        InitUniformBuffers();
        Gl.Enable(GLEnum.CullFace);
        Gl.CullFace(GLEnum.Front);
        Gl.FrontFace(GLEnum.CW);
    }

    private void EnableAntiAliasing()
    {
        Gl.Enable(GLEnum.Multisample);
    }

    private void EnableBlending()
    {
        Gl.Enable(GLEnum.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void OnUpdate(double deltaTime)
    {
        if (!running)
        {
            CloseWindow();
            return;
        }

        imGuiController.Update((float)deltaTime);
        OnUpdateEvent?.Invoke(deltaTime);
        //        game.Update(deltaTime);
    }

    private void OnRender(double delta)
    {
        _elapsedTime += delta;


        if (_elapsedTime >= 1.0)
        {
            _fps = _frameCount / _elapsedTime;
            _frameCount = 0;
            _elapsedTime = 0;
        }

        _frameCount++;
        //   Gl.Enable(EnableCap.DepthTest);
        Gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
       // EnableBlending();
        Gl.Enable(EnableCap.DepthTest);

        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        OnRenderEvent?.Invoke(delta, Gl);
        // game.Draw(Gl, delta);
        // game.DrawUi(Gl, delta);

        imGuiController.Render();
    }


    private void FrameBufferResize(Vector2D<int> size)
    {
        Gl.Viewport(size);
    }

    private void OnClose()
    {
        imGuiController?.Dispose();
        Input?.Dispose();
        Gl?.Dispose();
    }

    public unsafe void SetCursorMode(CursorModeValue cursorMode)
    {
        glfw.SetInputMode((WindowHandle*)Window.Handle, CursorStateAttribute.Cursor, cursorMode);
    }

    public bool CursorIsNotAvailable() => GetCursorMode() != CursorModeValue.CursorNormal;

    public unsafe CursorModeValue GetCursorMode()
    {
        return (CursorModeValue)glfw
            .GetInputMode((WindowHandle*)Window.Handle, CursorStateAttribute.Cursor);
    }


    public Vector2Int GetWindowSize()
    {
        return new Vector2Int(Window.Size.X, Window.Size.Y);
    }

    private void CloseWindow()
    {
        Window.Close();

        Window.Dispose();
    }

    private void KeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        if (key == Key.F1)
        {
            unsafe
            {
                SetCursorMode(
                    (GetCursorMode() == CursorModeValue.CursorNormal)
                        ? CursorModeValue.CursorDisabled
                        : CursorModeValue.CursorNormal
                );
            }
        }
    }
}
