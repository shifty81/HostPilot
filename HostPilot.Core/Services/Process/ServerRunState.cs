namespace HostPilot.Core.Services.Process;

public sealed class ServerRunState
{
    public bool IsRunning { get; set; }
    public int? ProcessId { get; set; }
    public DateTime? StartedUtc { get; set; }
    public DateTime? StoppedUtc { get; set; }
    public string StatusText { get; set; } = "Stopped";
}
