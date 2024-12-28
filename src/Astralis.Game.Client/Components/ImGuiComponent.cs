using Astralis.Game.Client.Components.Base;
using Astralis.Game.Client.Components.Ecs;
using ImGuiNET;

namespace Astralis.Game.Client.Components;

public class ImGuiComponent : BaseGameObject ,IImGuiComponent
{
    public string Id { get; set; }
    public void Render()
    {
        ImGui.Begin("Test");

        ImGui.Text("Hello, world!");

        ImGui.End();
    }


}
