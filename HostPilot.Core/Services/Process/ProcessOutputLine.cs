namespace HostPilot.Core.Services.Process;

public sealed class ProcessOutputLine
{
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string Stream { get; set; } = "stdout";
    public string Text { get; set; } = "";
}
