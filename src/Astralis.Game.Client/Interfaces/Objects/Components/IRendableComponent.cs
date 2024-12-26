using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Interfaces.Objects.Components;

public interface IRendableComponent
{
    void Render(GL gl);
}
