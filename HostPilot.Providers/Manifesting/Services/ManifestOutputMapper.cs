using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Enums;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ManifestOutputMapper : IManifestOutputMapper
{
    public ManifestOutputMap BuildOutput(ProviderManifest manifest, IReadOnlyDictionary<string, object?> state)
    {
        var map = new ManifestOutputMap();

        foreach (var field in manifest.Fields)
        {
            state.TryGetValue(field.Key, out var value);
            if (value is null)
            {
                continue;
            }

            foreach (var target in field.Output.Targets)
            {
                switch (target.Type)
                {
                    case ManifestOutputTargetType.LaunchArg:
                        map.LaunchArguments.Add($"{target.ArgName} {FormatValue(value)}");
                        break;
                    case ManifestOutputTargetType.CfgKey:
                        AddFileValue(map.CfgFiles, target.File ?? "server.cfg", target.Path ?? field.Key, value);
                        break;
                    case ManifestOutputTargetType.JsonPath:
                        AddFileValue(map.JsonFiles, target.File ?? "config.json", target.Path ?? field.Key, value);
                        break;
                    case ManifestOutputTargetType.EnvironmentVariable:
                        if (!string.IsNullOrWhiteSpace(target.EnvironmentVariable))
                        {
                            map.EnvironmentVariables[target.EnvironmentVariable] = Convert.ToString(value) ?? string.Empty;
                        }
                        break;
                }
            }
        }

        return map;
    }

    private static void AddFileValue(Dictionary<string, Dictionary<string, object?>> fileMap, string file, string path, object? value)
    {
        if (!fileMap.TryGetValue(file, out var values))
        {
            values = new Dictionary<string, object?>();
            fileMap[file] = values;
        }

        values[path] = value;
    }

    private static string FormatValue(object value)
    {
        return value is string text && text.Contains(' ')
            ? $"\"{text}\""
            : Convert.ToString(value) ?? string.Empty;
    }
}
