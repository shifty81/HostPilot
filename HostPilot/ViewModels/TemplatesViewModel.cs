using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using HostPilot.Core.Models.Templates;

namespace HostPilot.ViewModels;

public sealed class TemplatesViewModel
{
    private readonly HttpClient _httpClient;

    public TemplatesViewModel(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public ObservableCollection<DeploymentTemplateBundle> Templates { get; } = new();

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var bundles = await _httpClient.GetFromJsonAsync<List<DeploymentTemplateBundle>>("/api/templates", cancellationToken)
            ?? new List<DeploymentTemplateBundle>();

        Templates.Clear();
        foreach (var bundle in bundles.OrderBy(x => x.DisplayName))
        {
            Templates.Add(bundle);
        }
    }
}
