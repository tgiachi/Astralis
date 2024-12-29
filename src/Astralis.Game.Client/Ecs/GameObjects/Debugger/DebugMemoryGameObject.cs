using Astralis.Game.Client.Components.Entities;
using Astralis.Game.Client.Ecs.GameObjects.Base;
using Humanizer;
using ImGuiNET;

namespace Astralis.Game.Client.Ecs.GameObjects.Debugger;

public class DebugMemoryGameObject : TimedGameObject, IDebuggableComponent
{
    private long _totalMemory;

    public DebugMemoryGameObject() : base(1.0)
    {
    }

    public string Name => "Memory";
    public string Category => "System";

    public void DebugRender()
    {
        ImGui.Text($"Total Memory: {_totalMemory.Bytes()}");
        ImGui.Text("FPS: " + (AstralisGameInstances.OpenGlContext.Fps).ToString("0.00"));
    }

    public override void Trigger()
    {
        _totalMemory = GC.GetTotalMemory(false);

        base.Trigger();
    }
}
