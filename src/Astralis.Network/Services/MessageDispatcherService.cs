using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astralis.Network.Interfaces.Listeners;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Types;
using Serilog;
using ListenerResult =
    System.Func<string, Astralis.Network.Interfaces.Messages.INetworkMessage, System.Threading.Tasks.ValueTask>;

namespace Astralis.Network.Services;

public class MessageDispatcherService : IMessageDispatcherService
{
    private readonly ILogger _logger = Log.ForContext<MessageDispatcherService>();
    private readonly IMessageTypesService _messageTypesService;
    private readonly INetworkMessageFactory _networkMessageFactory;


    private readonly IMessageChannelService _messageChannelService;

    private readonly Task _dispatchIncomingMessagesTask;

    private readonly CancellationTokenSource _incomingMessagesCancellationTokenSource = new();

    private readonly ConcurrentDictionary<NetworkMessageType, List<ListenerResult>> _handlers = new();

    public MessageDispatcherService(
        IMessageTypesService messageTypesService, INetworkMessageFactory networkMessageFactory,
        IMessageChannelService messageChannelService
    )
    {
        _messageTypesService = messageTypesService;

        _networkMessageFactory = networkMessageFactory;
        _messageChannelService = messageChannelService;

        _dispatchIncomingMessagesTask = DispatchIncomingMessages();
    }

    private async Task DispatchIncomingMessages()
    {
        while (!_incomingMessagesCancellationTokenSource.Token.IsCancellationRequested)
        {
            await foreach (var message in _messageChannelService.IncomingReaderChannel.ReadAllAsync(
                               _incomingMessagesCancellationTokenSource.Token
                           ))
            {
                var parsedMessage = await _networkMessageFactory.ParseAsync(message.Packet);

                DispatchMessage(message.SessionId, parsedMessage);
            }
        }
    }

    public void RegisterMessageListener<TMessage>(INetworkMessageListener<TMessage> listener)
        where TMessage : class, INetworkMessage
    {
        RegisterMessageListener<TMessage>(
            async (sessionId, message) => await listener.OnMessageReceivedAsync(sessionId, message)
        );
    }


    public void RegisterMessageListener<TMessage>(
        Func<string, TMessage, ValueTask> listener
    ) where TMessage : class, INetworkMessage
    {
        var messageType = _messageTypesService.GetMessageType(typeof(TMessage));

        if (!_handlers.TryGetValue(messageType, out var handlers))
        {
            handlers = new List<Func<string, INetworkMessage, ValueTask>>();
            _handlers.TryAdd(messageType, handlers);
        }

        handlers.Add(
            async (sessionId, message) =>
            {
                if (message is TMessage typedMessage)
                {
                    await listener.Invoke(sessionId, typedMessage);
                }
            }
        );
    }

    public async void DispatchMessage<TMessage>(string sessionId, TMessage message) where TMessage : class, INetworkMessage
    {
        var messageType = _messageTypesService.GetMessageType(message.GetType());

        if (!_handlers.TryGetValue(messageType, out var handlers))
        {
            _logger.Warning("No handlers registered for message type {messageType}", messageType);
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.Invoke(sessionId, message);
        }
    }

    public void Dispose()
    {
        _incomingMessagesCancellationTokenSource.Dispose();
    }
}
