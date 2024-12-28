using Astralis.Game.Client.Components.Ecs;
using ImGuiNET;

namespace Astralis.Game.Client.Components;

public class ImGuiComponent : IImGuiComponent
{
    public void Render()
    {
        ImGui.Begin("Test");

        ImGui.Text("Hello, world!");

        ImGui.End();
    }
}
