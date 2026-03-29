namespace HostPilot.Core.Services.Process;

public sealed class ProcessSupervisorProfile
{
    public string ProfileId { get; set; } = Guid.NewGuid().ToString("N");
    public string DisplayName { get; set; } = "New Server";
    public string InstallRoot { get; set; } = @"C:\Servers\Example";
    public string ServerExecutable { get; set; } = "server.exe";
    public string LaunchArguments { get; set; } = "";
    public string? WorkingDirectory { get; set; }
    public bool AutoRestartOnCrash { get; set; } = true;
    public int RestartDelaySeconds { get; set; } = 5;
    public string ProviderType { get; set; } = "SteamCmd";
    public string? SteamAppId { get; set; } = "123456";
}
