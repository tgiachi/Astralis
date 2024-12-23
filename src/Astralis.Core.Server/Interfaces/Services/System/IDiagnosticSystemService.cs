using Astralis.Core.Interfaces.Services.Base;

namespace Astralis.Core.Server.Interfaces.Services.System;

public interface IDiagnosticSystemService : IAstralisSystemService
{

    string PidFileName { get; }
}
