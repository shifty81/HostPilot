using System;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Contracts;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Services;

public sealed class AutomationScheduler : IDisposable
{
    private readonly Timer _timer;
    private readonly IAutomationEventBus _eventBus;

    public AutomationScheduler(IAutomationEventBus eventBus)
    {
        _eventBus = eventBus;
        _timer = new Timer(OnTimer, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public void Start(TimeSpan interval)
    {
        _timer.Change(interval, interval);
    }

    public void Stop()
    {
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    private async void OnTimer(object? state)
    {
        try
        {
            var scheduledEvent = new RuntimeEvent
            {
                EventType = "ScheduledTimeReached",
                Source = "AutomationScheduler",
                Severity = RuntimeEventSeverities.Info,
                ServerId = string.Empty
            };

            await _eventBus.PublishAsync(scheduledEvent);
        }
        catch
        {
            // Route to logging service later.
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
