namespace Astralis.Game.Client.Components.Entities;

public interface IDebuggableComponent
{
    string Name { get; }
    string Category { get; }
    void DebugRender();
}
