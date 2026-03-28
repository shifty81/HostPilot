using System.Collections.Concurrent;
using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Services;

public sealed class OperationScheduler
{
    private readonly object _sync = new();
    private readonly PriorityQueue<OperationRequest, int> _queue = new();
    private readonly ConcurrentDictionary<string, byte> _lockedTargets = new(StringComparer.OrdinalIgnoreCase);

    public void Enqueue(OperationRequest request)
    {
        lock (_sync)
        {
            _queue.Enqueue(request, -1 * (int)request.Priority);
        }
    }

    public bool TryAcquireNext(out OperationRequest? request)
    {
        lock (_sync)
        {
            if (_queue.Count == 0)
            {
                request = null;
                return false;
            }

            var skipped = new List<(OperationRequest Request, int Priority)>();
            request = null;

            while (_queue.Count > 0)
            {
                var next = _queue.Dequeue();
                var priority = -1 * (int)next.Priority;

                if (_lockedTargets.TryAdd(next.TargetId, 0))
                {
                    request = next;
                    break;
                }

                skipped.Add((next, priority));
            }

            foreach (var item in skipped)
                _queue.Enqueue(item.Request, item.Priority);

            return request is not null;
        }
    }

    public void ReleaseTarget(string targetId)
    {
        _lockedTargets.TryRemove(targetId, out _);
    }
}
