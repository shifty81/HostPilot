using System.Text.Json;
using HostPilot.Core.Models.Nodes;

namespace HostPilot.Core.Services.Nodes;

public sealed class FileNodeRegistryService : INodeRegistryService
{
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public FileNodeRegistryService(string path)
    {
        _path = path;
    }

    public async Task<IReadOnlyList<ManagedNode>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadAllUnsafeAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<ManagedNode?> GetAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.FirstOrDefault(x => x.Id == nodeId);
    }

    public async Task<ManagedNode> UpsertAsync(ManagedNode node, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var all = await ReadAllUnsafeAsync(cancellationToken);
            var existing = all.FirstOrDefault(x => x.Id == node.Id);
            if (existing is null)
            {
                all.Add(node);
            }
            else
            {
                var index = all.IndexOf(existing);
                all[index] = node;
            }

            await WriteAllUnsafeAsync(all, cancellationToken);
            return node;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var all = await ReadAllUnsafeAsync(cancellationToken);
            all.RemoveAll(x => x.Id == nodeId);
            await WriteAllUnsafeAsync(all, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task UpdateHeartbeatAsync(NodeHeartbeat heartbeat, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var all = await ReadAllUnsafeAsync(cancellationToken);
            var node = all.FirstOrDefault(x => x.Id == heartbeat.NodeId);
            if (node is null)
            {
                return;
            }

            node.LastHeartbeatUtc = heartbeat.TimestampUtc;
            node.CpuPercent = heartbeat.CpuPercent;
            node.MemoryUsedMb = heartbeat.MemoryUsedMb;
            node.MemoryTotalMb = heartbeat.MemoryTotalMb;
            node.AgentVersion = heartbeat.AgentVersion;
            node.Status = heartbeat.Status;

            await WriteAllUnsafeAsync(all, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<ManagedNode>> ReadAllUnsafeAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
        {
            return new List<ManagedNode>();
        }

        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<List<ManagedNode>>(stream, _json, cancellationToken) ?? new List<ManagedNode>();
    }

    private async Task WriteAllUnsafeAsync(List<ManagedNode> nodes, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, nodes, _json, cancellationToken);
    }
}
