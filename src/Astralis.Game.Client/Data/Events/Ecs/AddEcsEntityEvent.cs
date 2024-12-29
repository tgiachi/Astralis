using Astralis.Core.Interfaces.Events;
using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Data.Events.Ecs;

public record AddEcsEntityEvent(IGameObject GameObject) : IAstralisEvent;

