using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using HostPilot.Core.Models.Templates;

namespace HostPilot.Core.Services.Templates;

public sealed class FileTemplateLibraryService : ITemplateLibraryService
{
    private readonly string _libraryPath;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public FileTemplateLibraryService(string libraryPath)
    {
        _libraryPath = libraryPath;
    }

    public async Task<IReadOnlyList<DeploymentTemplateBundle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_libraryPath);
        var list = new List<DeploymentTemplateBundle>();

        foreach (var path in Directory.EnumerateFiles(_libraryPath, "*.json", SearchOption.TopDirectoryOnly))
        {
            await using var stream = File.OpenRead(path);
            var model = await JsonSerializer.DeserializeAsync<DeploymentTemplateBundle>(stream, _json, cancellationToken);
            if (model is not null)
            {
                list.Add(model);
            }
        }

        return list.OrderBy(x => x.DisplayName).ToList();
    }

    public async Task<DeploymentTemplateBundle> SaveAsync(DeploymentTemplateBundle bundle, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_libraryPath);
        bundle.UpdatedUtc = DateTimeOffset.UtcNow;
        bundle.Signature = new TemplateSignature
        {
            Algorithm = "SHA256",
            Checksum = ComputeChecksum(bundle)
        };

        var path = Path.Combine(_libraryPath, $"{bundle.Id}.json");
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, bundle, _json, cancellationToken);
        return bundle;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_libraryPath, $"{id}.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public async Task<string> ExportAsync(Guid id, string targetDirectory, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        var bundle = all.First(x => x.Id == id);

        Directory.CreateDirectory(targetDirectory);
        var tempDir = Path.Combine(targetDirectory, bundle.Slug + "_export");
        Directory.CreateDirectory(tempDir);

        var manifestPath = Path.Combine(tempDir, "template.json");
        await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(bundle, _json), cancellationToken);

        var zipPath = Path.Combine(targetDirectory, $"{bundle.Slug}.hostpilot-template.zip");
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        ZipFile.CreateFromDirectory(tempDir, zipPath);
        Directory.Delete(tempDir, true);
        return zipPath;
    }

    public async Task<DeploymentTemplateBundle> ImportAsync(string bundlePath, CancellationToken cancellationToken = default)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "hostpilot_tpl_" + Guid.NewGuid().ToString("N"));
        ZipFile.ExtractToDirectory(bundlePath, tempDir);

        var manifestPath = Path.Combine(tempDir, "template.json");
        var json = await File.ReadAllTextAsync(manifestPath, cancellationToken);
        var bundle = JsonSerializer.Deserialize<DeploymentTemplateBundle>(json, _json)
            ?? throw new InvalidOperationException("Invalid template bundle.");

        Directory.Delete(tempDir, true);
        return await SaveAsync(bundle, cancellationToken);
    }

    public Task<TemplateReviewResult> ReviewAsync(DeploymentTemplateBundle bundle, CancellationToken cancellationToken = default)
    {
        var result = new TemplateReviewResult
        {
            CanApply = true
        };

        if (string.IsNullOrWhiteSpace(bundle.GameId))
        {
            result.CanApply = false;
            result.Errors.Add("Template is missing GameId.");
        }

        if (bundle.Signature is null)
        {
            result.Warnings.Add("Template has no signature.");
        }

        if (bundle.Mods.Count == 0)
        {
            result.Notices.Add("Template has no mods.");
        }

        return Task.FromResult(result);
    }

    private string ComputeChecksum(DeploymentTemplateBundle bundle)
    {
        var json = JsonSerializer.Serialize(bundle, _json);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json)));
    }
}
