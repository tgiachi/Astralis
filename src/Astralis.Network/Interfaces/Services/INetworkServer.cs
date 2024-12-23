using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astralis.Core.Interfaces.Services.Base;
using Astralis.Network.Data.Internal;
using Astralis.Network.Interfaces.Listeners;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Metrics.Server;

namespace Astralis.Network.Interfaces.Services;

public interface INetworkServer : IAstralisSystemService, IDisposable
{
    bool IsRunning { get; }

    void RegisterMessageListener<TMessage>(INetworkMessageListener<TMessage> listener)
        where TMessage : class, INetworkMessage;

    void RegisterMessageListener<TMessage>(Func<string, TMessage, ValueTask> listener)
        where TMessage : class, INetworkMessage;

    ValueTask BroadcastMessageAsync(INetworkMessage message);

    ValueTask SendMessagesAsync(IEnumerable<SessionNetworkMessage> messages);
    ValueTask SendMessageAsync(SessionNetworkMessage messages);

    ValueTask SendMessageAsync(string sessionId, INetworkMessage message);

    IObservable<NetworkMetricsSnapshot> MetricsObservable { get; }
}
