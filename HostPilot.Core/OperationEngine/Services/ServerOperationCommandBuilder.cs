using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Services;

/// <summary>
/// Convenience factory that creates typed <see cref="OperationRequest"/> objects
/// for the most common server lifecycle operations.
/// </summary>
public sealed class ServerOperationCommandBuilder
{
    public OperationRequest BuildInstall(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.InstallServer, serverName, metadata);

    public OperationRequest BuildUpdate(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.UpdateServer, serverName, metadata);

    public OperationRequest BuildValidate(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.ValidateServer, serverName, metadata);

    public OperationRequest BuildStart(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.StartServer, serverName, metadata);

    public OperationRequest BuildStop(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.StopServer, serverName, metadata);

    public OperationRequest BuildRestart(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.RestartServer, serverName, metadata);

    public OperationRequest BuildBackup(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.BackupServer, serverName, metadata);

    public OperationRequest BuildInstallMods(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.InstallMods, serverName, metadata);

    public OperationRequest BuildImportExisting(string serverName, IReadOnlyDictionary<string, string>? metadata = null)
        => Build(OperationType.ImportExistingServer, serverName, metadata);

    private static OperationRequest Build(string type, string serverName, IReadOnlyDictionary<string, string>? metadata)
        => new()
        {
            Type = type,
            TargetId = serverName,
            Metadata = metadata ?? new Dictionary<string, string>(),
        };
}
