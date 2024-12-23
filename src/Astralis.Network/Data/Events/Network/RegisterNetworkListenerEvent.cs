using System;
using System.Threading.Tasks;
using Astralis.Core.Interfaces.Events;
using Astralis.Network.Interfaces.Messages;

namespace Astralis.Network.Data.Events.Network;

public class RegisterNetworkListenerEvent<TMessage> : IAstralisEvent where TMessage : class, INetworkMessage
{
    public Func<string, TMessage, ValueTask> Listener { get; }

    public RegisterNetworkListenerEvent(Func<string, TMessage, ValueTask> listener)
    {
        Listener = listener;
    }
}
