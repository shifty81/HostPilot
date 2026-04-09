using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using HostPilot.Core.Models.Nodes;

namespace HostPilot.ViewModels;

public sealed class NodesViewModel
{
    private readonly HttpClient _httpClient;

    public NodesViewModel(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public ObservableCollection<ManagedNode> Nodes { get; } = new();

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var nodes = await _httpClient.GetFromJsonAsync<List<ManagedNode>>("/api/nodes", cancellationToken)
            ?? new List<ManagedNode>();

        Nodes.Clear();
        foreach (var node in nodes.OrderBy(x => x.DisplayName))
        {
            Nodes.Add(node);
        }
    }

    public async Task<string> CreateEnrollmentTokenAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/nodes/{nodeId}/enrollment-token", new { }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<NodeEnrollmentToken>(cancellationToken: cancellationToken);
        return data?.Token ?? string.Empty;
    }
}
