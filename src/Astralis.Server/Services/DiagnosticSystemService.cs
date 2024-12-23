using System.Diagnostics;
using Astralis.Core.Interfaces.EventBus;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Events.Engine;
using Astralis.Core.Server.Events.Scheduler;
using Astralis.Core.Server.Interfaces.Services.System;
using Humanizer;
using Serilog;

namespace Astralis.Server.Services;

public class DiagnosticSystemService
    : IDiagnosticSystemService, IEventBusListener<EngineStartedEvent>, IEventBusListener<EngineShuttingDownEvent>
{
    private readonly ILogger _logger = Log.Logger.ForContext<DiagnosticSystemService>();
    private readonly IEventBusService _eventBusService;

    public string PidFileName { get; }

    private int _printCounter;

    private int _printInterval = 120;

    public DiagnosticSystemService(IEventBusService eventBusService, DirectoriesConfig directoriesConfig)
    {
        _eventBusService = eventBusService;
        PidFileName = Path.Combine(directoriesConfig.Root, "orionserver.pid");
    }

    public async Task StartAsync()
    {
        _eventBusService.Subscribe<EngineStartedEvent>(this);
        _eventBusService.Subscribe<EngineShuttingDownEvent>(this);

        await _eventBusService.PublishAsync(
            new AddSchedulerJobEvent("PrintDiagnosticInfo", TimeSpan.FromMinutes(1), PrintDiagnosticInfoAsync)
        );
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public Task OnEventAsync(EngineStartedEvent message)
    {
        File.WriteAllText(PidFileName, Environment.ProcessId.ToString());

        return Task.CompletedTask;
    }

    private Task PrintDiagnosticInfoAsync()
    {
        using var currentProcess = Process.GetCurrentProcess();

        _logger.Information(
            "Memory usage private: {Private} Paged: {Paged} Total Threads: {Threads} PID: {Pid}",
            currentProcess.WorkingSet64.Bytes(),
            GC.GetTotalMemory(false).Bytes(),
            currentProcess.Threads.Count,
            currentProcess.Id
        );

        _printCounter++;

        if (_printCounter % _printInterval == 0)
        {
            _logger.Information("GC Memory: {Memory}", GC.GetTotalMemory(false).Bytes());
            _printCounter = 0;
        }


        return Task.CompletedTask;
    }

    public Task OnEventAsync(EngineShuttingDownEvent message)
    {
        if (File.Exists(PidFileName))
        {
            File.Delete(PidFileName);
        }

        return Task.CompletedTask;
    }
}
