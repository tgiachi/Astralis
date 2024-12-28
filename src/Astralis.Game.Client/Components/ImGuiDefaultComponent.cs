using Astralis.Game.Client.Components.Ecs;
using ImGuiNET;

namespace Astralis.Game.Client.Components;

public class ImGuiDefaultComponent : IImGuiComponent
{
    public void Render()
    {
        ImGui.Begin("Test");

        ImGui.Text("Hello, world!");
        ImGui.ShowMetricsWindow();
        ImGui.ShowDebugLogWindow( );

        ImGui.End();
    }

    public object[] GetComponents()
    {
        return [(IImGuiComponent)this];
    }
}
