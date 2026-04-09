using System;
using System.Collections.ObjectModel;

namespace HostPilot.ViewModels;

public sealed class RolloutDashboardViewModel
{
    public ObservableCollection<RolloutWaveViewModel> Waves { get; } = new();
    public ObservableCollection<RolloutNodeProgressViewModel> ActiveNodes { get; } = new();
}

public sealed class RolloutWaveViewModel
{
    public string WaveName { get; set; } = string.Empty;
    public int Concurrency { get; set; }
    public bool RequiresApproval { get; set; }
}

public sealed class RolloutNodeProgressViewModel
{
    public string NodeId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Percent { get; set; }
    public string Deployment { get; set; } = string.Empty;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
