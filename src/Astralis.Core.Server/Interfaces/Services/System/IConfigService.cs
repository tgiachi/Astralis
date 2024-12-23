using Astralis.Core.Interfaces.Services.Base;

namespace Astralis.Core.Server.Interfaces.Services.System;

public interface IConfigService : IAstralisSystemService
{
    void SearchForConfigAttributes(object instance);
}
