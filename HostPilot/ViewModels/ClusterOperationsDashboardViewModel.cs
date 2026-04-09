using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HostPilot.ViewModels;

public sealed class ClusterOperationsDashboardViewModel : INotifyPropertyChanged
{
    private string _clusterStatus = "Unknown";
    private int _onlineNodes;
    private int _runningJobs;
    private int _alerts;

    public string ClusterStatus
    {
        get => _clusterStatus;
        set { _clusterStatus = value; OnPropertyChanged(); }
    }

    public int OnlineNodes
    {
        get => _onlineNodes;
        set { _onlineNodes = value; OnPropertyChanged(); }
    }

    public int RunningJobs
    {
        get => _runningJobs;
        set { _runningJobs = value; OnPropertyChanged(); }
    }

    public int Alerts
    {
        get => _alerts;
        set { _alerts = value; OnPropertyChanged(); }
    }

    public ObservableCollection<NodeTileViewModel> Nodes { get; } = new();
    public ObservableCollection<ExecutionEventViewModel> Events { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class NodeTileViewModel : INotifyPropertyChanged
{
    private string _displayName = string.Empty;
    private string _health = "Unknown";
    private double _cpu;
    private double _memory;

    public string DisplayName
    {
        get => _displayName;
        set { _displayName = value; OnPropertyChanged(); }
    }

    public string Health
    {
        get => _health;
        set { _health = value; OnPropertyChanged(); }
    }

    public double Cpu
    {
        get => _cpu;
        set { _cpu = value; OnPropertyChanged(); }
    }

    public double Memory
    {
        get => _memory;
        set { _memory = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class ExecutionEventViewModel
{
    public string Timestamp { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string WorkItemId { get; init; } = string.Empty;
    public string NodeId { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
