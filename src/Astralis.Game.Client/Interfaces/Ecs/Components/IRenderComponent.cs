using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Interfaces.Ecs.Components;

public interface IRenderComponent
{
    void Render(GL gl);
}
