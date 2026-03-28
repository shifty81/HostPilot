using System.Text.Json;
using HostPilot.Core.Models.FirstRun;

namespace HostPilot.Core.Services.Startup;

public sealed class JsonFirstRunStateStore : IFirstRunStateStore
{
    private readonly string _path;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonFirstRunStateStore(string path)
    {
        _path = path;
    }

    public FirstRunWizardState Load()
    {
        if (!File.Exists(_path))
        {
            return new FirstRunWizardState();
        }

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<FirstRunWizardState>(json, JsonOptions) ?? new FirstRunWizardState();
    }

    public void Save(FirstRunWizardState state)
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(state, JsonOptions);
        File.WriteAllText(_path, json);
    }
}
