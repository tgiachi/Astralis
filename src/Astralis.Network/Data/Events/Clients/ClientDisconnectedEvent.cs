using Astralis.Core.Interfaces.Events;

namespace Astralis.Network.Data.Events.Clients;

public class ClientDisconnectedEvent(string SessionId) : IAstralisEvent;
