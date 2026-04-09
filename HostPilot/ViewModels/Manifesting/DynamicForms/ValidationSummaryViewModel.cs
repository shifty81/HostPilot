using System.Collections.ObjectModel;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.UI.DynamicForms.ViewModels;

public sealed class ValidationSummaryViewModel : ObservableObject
{
    public ObservableCollection<ManifestValidationIssue> Issues { get; } = [];
    public int ErrorCount => Issues.Count(x => string.Equals(x.Severity, "Error", StringComparison.OrdinalIgnoreCase));
}
