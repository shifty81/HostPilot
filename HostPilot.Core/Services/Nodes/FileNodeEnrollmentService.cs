using System.Security.Cryptography;
using System.Text.Json;
using HostPilot.Core.Models.Nodes;

namespace HostPilot.Core.Services.Nodes;

public sealed class FileNodeEnrollmentService : INodeEnrollmentService
{
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public FileNodeEnrollmentService(string path)
    {
        _path = path;
    }

    public async Task<NodeEnrollmentToken> CreateEnrollmentTokenAsync(Guid nodeId, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var token = new NodeEnrollmentToken
        {
            NodeId = nodeId,
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            ExpiresUtc = DateTimeOffset.UtcNow.Add(ttl),
            Consumed = false
        };

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var all = await ReadAllUnsafeAsync(cancellationToken);
            all.RemoveAll(x => x.NodeId == nodeId && !x.Consumed);
            all.Add(token);
            await WriteAllUnsafeAsync(all, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        return token;
    }

    public async Task<bool> ValidateAndConsumeTokenAsync(Guid nodeId, string token, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var all = await ReadAllUnsafeAsync(cancellationToken);
            var item = all.FirstOrDefault(x => x.NodeId == nodeId && x.Token == token && !x.Consumed);
            if (item is null || item.ExpiresUtc < DateTimeOffset.UtcNow)
            {
                return false;
            }

            item.Consumed = true;
            await WriteAllUnsafeAsync(all, cancellationToken);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<NodeEnrollmentToken>> ReadAllUnsafeAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
        {
            return new List<NodeEnrollmentToken>();
        }

        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<List<NodeEnrollmentToken>>(stream, _json, cancellationToken)
            ?? new List<NodeEnrollmentToken>();
    }

    private async Task WriteAllUnsafeAsync(List<NodeEnrollmentToken> tokens, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, tokens, _json, cancellationToken);
    }
}
