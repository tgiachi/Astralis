using Arch.Core;
using Arch.Core.Extensions;
using Arch.Core.Utils;
using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Components.Base;

public class BaseGameObject : IGameObject
{
    public string Id { get; set; }

    public Entity Entity { get; set; }


}
