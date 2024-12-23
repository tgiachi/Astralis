using System;
using System.Collections.Generic;
using Astralis.Network.Interfaces.Sessions;

namespace Astralis.Network.Interfaces.Services;

public interface INetworkSessionService
{
    List<string> GetSessionIds { get; }
    ISessionObject? GetSessionObject(string sessionId);
    void AddSession(string sessionId, ISessionObject sessionObject);
    void RemoveSession(string sessionId);
    void UpdateLastActive(string sessionId);

    IEnumerable<ISessionObject> GetExpiredSessions(TimeSpan expirationTime);

    int SessionCount { get; }
}
