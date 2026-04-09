namespace HostPilot.Remote.Execution.Services;

using HostPilot.Remote.Contracts.Models;

public sealed class RemoteCommandCatalog
{
    private readonly Dictionary<string, RemoteCommandDescriptor> _commands = new(StringComparer.OrdinalIgnoreCase)
    {
        ["server.install"] = new() { CommandKey = "server.install", DisplayName = "Install Server", HandlerKey = "install", RiskLevel = "Medium" },
        ["server.start"] = new() { CommandKey = "server.start", DisplayName = "Start Server", HandlerKey = "start", RiskLevel = "Low" },
        ["server.stop"] = new() { CommandKey = "server.stop", DisplayName = "Stop Server", HandlerKey = "stop", RiskLevel = "Medium" },
        ["server.restart"] = new() { CommandKey = "server.restart", DisplayName = "Restart Server", HandlerKey = "restart", RiskLevel = "Medium" },
        ["server.backup.create"] = new() { CommandKey = "server.backup.create", DisplayName = "Create Backup", HandlerKey = "backup", RiskLevel = "Low" },
        ["node.agent.restart"] = new() { CommandKey = "node.agent.restart", DisplayName = "Restart Agent", HandlerKey = "agent-restart", RiskLevel = "High" },
    };

    public bool TryGet(string commandKey, out RemoteCommandDescriptor descriptor) => _commands.TryGetValue(commandKey, out descriptor!);
    public IReadOnlyCollection<RemoteCommandDescriptor> GetAll() => _commands.Values.ToArray();
}
