using System.Text.Json;
using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public class InstalledModRegistryService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    public List<InstalledModRecord> Load(string registryPath)
    {
        if (!File.Exists(registryPath)) return new();
        var json = File.ReadAllText(registryPath);
        return JsonSerializer.Deserialize<List<InstalledModRecord>>(json, _jsonOptions) ?? new();
    }

    public void Save(string registryPath, IEnumerable<InstalledModRecord> records)
    {
        var dir = Path.GetDirectoryName(registryPath);
        if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(records, _jsonOptions);
        File.WriteAllText(registryPath, json);
    }

    public InstalledModRecord Upsert(List<InstalledModRecord> records, InstalledModRecord record)
    {
        var existing = records.FirstOrDefault(x => string.Equals(x.ServerName, record.ServerName, StringComparison.OrdinalIgnoreCase)
                                                && string.Equals(x.InstalledPath, record.InstalledPath, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
        {
            records.Add(record);
            return record;
        }

        existing.Name = record.Name;
        existing.Version = record.Version;
        existing.Source = record.Source;
        existing.Provider = record.Provider;
        existing.SourcePath = record.SourcePath;
        existing.HashSha256 = record.HashSha256;
        existing.InstalledAtUtc = record.InstalledAtUtc;
        existing.RequiresRestart = record.RequiresRestart;
        existing.Managed = record.Managed;
        return existing;
    }
}
