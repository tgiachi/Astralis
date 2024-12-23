using System;
using Astralis.Network.Interfaces.Messages;

namespace Astralis.Network.Data.Events.Clients;

public class ClientLoggedEvent(string SessionId, Guid UserId) : INetworkMessage;
