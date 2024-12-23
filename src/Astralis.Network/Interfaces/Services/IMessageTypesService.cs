using System;
using Astralis.Network.Types;

namespace Astralis.Network.Interfaces.Services;

public interface IMessageTypesService
{
    Type GetMessageType(NetworkMessageType messageType);
    NetworkMessageType GetMessageType(Type type);

    NetworkMessageType GetMessageType<T>() where T : class;

    void RegisterMessageType(NetworkMessageType messageType, Type type);
}
