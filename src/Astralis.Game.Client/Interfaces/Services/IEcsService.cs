using Arch.Core;
using Astralis.Core.Interfaces.Services.Base;
using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Interfaces.Services;

public interface IEcsService : IAstralisSystemService, IDisposable
{
    void AddEntity<TGameObject>(TGameObject gameObject) where TGameObject : IGameObject;
}
