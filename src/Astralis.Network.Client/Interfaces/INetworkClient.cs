using System;
using System.Threading.Tasks;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Types;

namespace Astralis.Network.Client.Interfaces;

public interface INetworkClient
{
    delegate void MessageReceivedEventHandler(NetworkMessageType messageType, INetworkMessage message);
    event MessageReceivedEventHandler MessageReceived;

    event EventHandler Connected;

    bool IsConnected { get; }

    void PoolEvents();

    void Connect(string host, int port);

    Task SendMessageAsync<T>(T message) where T : class, INetworkMessage;

    public IObservable<T> SubscribeToMessage<T>() where T : class, INetworkMessage;
}
