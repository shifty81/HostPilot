using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HostPilot.Core.Models.Discovery;

namespace HostPilot.ViewModels;

public sealed class ImportDiscoveredServersViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ImportCandidateItemViewModel> Candidates { get; } = new();

    public int SelectedCount => Candidates.Count(x => x.IsSelected && !x.Candidate.IsImported);

    public void Load(IEnumerable<DiscoveredServerCandidate> candidates)
    {
        Candidates.Clear();
        foreach (var candidate in candidates)
        {
            var item = new ImportCandidateItemViewModel(candidate);
            item.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(ImportCandidateItemViewModel.IsSelected))
                {
                    OnPropertyChanged(nameof(SelectedCount));
                }
            };
            Candidates.Add(item);
        }

        OnPropertyChanged(nameof(SelectedCount));
    }

    public IReadOnlyList<DiscoveredServerCandidate> SelectedForImport()
        => Candidates
            .Where(x => x.IsSelected && !x.Candidate.IsImported)
            .Select(x => x.Candidate)
            .ToList();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class ImportCandidateItemViewModel : INotifyPropertyChanged
{
    private bool _isSelected;

    public ImportCandidateItemViewModel(DiscoveredServerCandidate candidate)
    {
        Candidate = candidate;
        _isSelected = !candidate.IsImported;
    }

    public DiscoveredServerCandidate Candidate { get; }
    public string DisplayName => Candidate.DisplayName;
    public string ServerType => Candidate.ServerType;
    public string InstallPath => Candidate.InstallPath;
    public string ExecutablePath => Candidate.ExecutablePath;
    public double Confidence => Candidate.Confidence;
    public string Evidence => Candidate.Evidence.Count == 0 ? "—" : string.Join(" | ", Candidate.Evidence);
    public bool IsAlreadyImported => Candidate.IsImported;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
