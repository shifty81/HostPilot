namespace HostPilot.Core.OperationEngine.Models;

public static class OperationType
{
    public const string InstallServer = "INSTALL_SERVER";
    public const string UpdateServer = "UPDATE_SERVER";
    public const string StartServer = "START_SERVER";
    public const string StopServer = "STOP_SERVER";
    public const string RestartServer = "RESTART_SERVER";
    public const string ValidateServer = "VALIDATE_SERVER";
    public const string BackupServer = "BACKUP_SERVER";
    public const string StartCluster = "START_CLUSTER";
    public const string StopCluster = "STOP_CLUSTER";
    public const string SyncClusterConfig = "SYNC_CLUSTER_CONFIG";
}
