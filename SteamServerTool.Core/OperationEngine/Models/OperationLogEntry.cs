namespace SteamServerTool.Core.OperationEngine.Models;

public sealed record OperationLogEntry(DateTimeOffset TimestampUtc, string Level, string Message);
