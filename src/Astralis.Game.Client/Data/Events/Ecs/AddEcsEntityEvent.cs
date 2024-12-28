using Astralis.Core.Interfaces.Events;

namespace Astralis.Game.Client.Data.Events.Ecs;

public record AddEcsEntityEvent(params object[] Components) : IAstralisEvent;

