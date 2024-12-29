using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Ecs.Components;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class DebugRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<DebuggableComponent>();

    public DebugRenderSystem(World world) : base(world)
    {
    }

    public override void Update(in GL t)
    {
        ImGui.Begin("Debugger");
        World.Query(
            in _desc,
            (ref Entity entity, ref DebuggableComponent debuggable) =>
            {
                ImGui.BeginGroup();
                ImGui.Separator();
                ImGui.Text(debuggable.Obj.Name + "_" + entity.Id);
                ImGui.Separator();
                debuggable.Obj.DebugRender();
                ImGui.EndGroup();
            }
        );

        ImGui.End();

        base.Update(in t);
    }
}
