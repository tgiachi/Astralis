using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Components.Ecs;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class DebugRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<IDebuggableComponent>();

    public DebugRenderSystem(World world) : base(world)
    {
    }

    public override void Update(in GL t)
    {
        ImGui.Begin("Debugger");
        World.Query(
            in _desc,
            (ref IDebuggableComponent debuggable) =>
            {
                ImGui.BeginGroup();
                ImGui.Separator();
                ImGui.Text(debuggable.Name + "_" + debuggable.Entity.Id);
                ImGui.Separator();
                debuggable.DebugRender();
                ImGui.EndGroup();
            }
        );

        ImGui.End();

        base.Update(in t);
    }
}
