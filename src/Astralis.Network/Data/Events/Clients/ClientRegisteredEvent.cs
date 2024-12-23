using System;
using Astralis.Core.Interfaces.Events;

namespace Astralis.Network.Data.Events.Clients;

public class ClientRegisteredEvent(Guid Id, string Username) : IAstralisEvent;
