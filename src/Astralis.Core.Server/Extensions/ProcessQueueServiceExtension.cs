using Astralis.Core.Server.Interfaces.Services.System;

namespace Astralis.Core.Server.Extensions;

public static class ProcessQueueServiceExtension
{
    public static string DefaultContext = "default";
    public static string WorldGenerationContext = "world_generation";

    public static Task<T> EnqueueDefault<T>(
        this IProcessQueueService processQueueService, Func<T> func, CancellationToken cancellationToken = default
    )
    {
        return processQueueService.Enqueue(DefaultContext, func, cancellationToken);
    }

    public static Task<T> EnqueueDefault<T>(
        this IProcessQueueService processQueueService, Func<Task<T>> func, CancellationToken cancellationToken = default
    )
    {
        return processQueueService.Enqueue(DefaultContext, func, cancellationToken);
    }

    public static Task EnqueueDefault(
        this IProcessQueueService processQueueService, Action action, CancellationToken cancellationToken = default
    )
    {
        return processQueueService.Enqueue(DefaultContext, action, cancellationToken);
    }

    public static Task EnqueueDefault(
        this IProcessQueueService processQueueService, Func<Task> func, CancellationToken cancellationToken = default
    )
    {
        return processQueueService.Enqueue(DefaultContext, func, cancellationToken);
    }
}
