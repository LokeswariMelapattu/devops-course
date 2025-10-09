using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class StartTimeHostedService : IHostedService
{
    public static DateTime StartTime { get; private set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartTime = DateTime.UtcNow;
        Console.WriteLine($"Service started at {StartTime}");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
