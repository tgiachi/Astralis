using Arch.Core;
using Astralis.Core.Interfaces.Services.Base;

namespace Astralis.Game.Client.Interfaces.Services;

public interface IEcsService : IAstralisSystemService, IDisposable
{
    Entity CreateEntity(params object[] components);
}
