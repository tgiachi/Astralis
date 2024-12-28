using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Services;
using Astralis.Core.Services;
using Astralis.Game.Client.Impl;
using Astralis.Game.Client.Interfaces.Services;
using Jab;

namespace Astralis.Game.Client;

[ServiceProvider]
[Singleton(typeof(IEventBusService), typeof(EventBusService))]
[Singleton(typeof(IVariablesService), typeof(VariablesService))]
[Singleton(typeof(IVersionService), typeof(VersionService))]
[Singleton(typeof(IEcsService), typeof(EcsService))]
[Singleton(typeof(IFontManagerService), typeof(FontManagerService))]
public partial class AstralisServiceProvider
{

}
