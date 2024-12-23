using System;
using System.Collections.Generic;
using System.Linq;
using Astralis.Network.Data.Internal;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Types;
using Serilog;

namespace Astralis.Network.Services;

public class MessageTypesService : IMessageTypesService
{
    private readonly ILogger _logger = Log.ForContext<MessageTypesService>();

    private readonly Dictionary<NetworkMessageType, Type> _messageTypes = new(new Dictionary<NetworkMessageType, Type>());

    private readonly Dictionary<Type, NetworkMessageType> _messageTypesReverse =
        new(new Dictionary<Type, NetworkMessageType>());

    public MessageTypesService(List<MessageTypeObject>? messageTypes = null)
    {
        if (messageTypes != null)
        {
            foreach (var messageType in messageTypes)
            {
                RegisterMessageType(messageType.MessageType, messageType.Type);
            }
        }
    }


    public Type GetMessageType(NetworkMessageType messageType)
    {
        if (!_messageTypes.TryGetValue(messageType, out var type))
        {
            _logger.Error("Message type {messageType} is not registered", messageType);
            throw new ArgumentException("Message type is not registered", nameof(messageType));
        }

        return type;
    }

    public NetworkMessageType GetMessageType(Type type)
    {
        if (!_messageTypesReverse.TryGetValue(type, out var messageType))
        {
            _logger.Error("Type {type} is not registered", type.Name);
            throw new ArgumentException("Type is not registered", nameof(type));
        }

        return messageType;
    }

    public NetworkMessageType GetMessageType<T>() where T : class
    {
        var messageType = _messageTypes.First(x => x.Value == typeof(T)).Key;

        return messageType;
    }

    public void RegisterMessageType(NetworkMessageType messageType, Type type)
    {
        if (!typeof(INetworkMessage).IsAssignableFrom(type))
        {
            _logger.Error("Type {type} does not implement INetworkMessage", type.Name);
            throw new ArgumentException("Type does not implement INetworkMessage", nameof(type));
        }

        if (_messageTypes.ContainsKey(messageType))
        {
            _logger.Error("Message type {messageType} is already registered", messageType);
            throw new ArgumentException("Message type is already registered", nameof(messageType));
        }


        _logger.Debug("Registered message type {messageType} with type {type}", messageType, type.Name);

        _messageTypes.Add(messageType, type);
        _messageTypesReverse.Add(type, messageType);
    }
}
