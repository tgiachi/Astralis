namespace Astralis.Game.Client.Components.Entities;

public interface IDebuggableComponent
{
    string Name { get; }
    void DebugRender();
}
