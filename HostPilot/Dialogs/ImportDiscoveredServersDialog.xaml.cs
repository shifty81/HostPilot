using System.Windows;
using HostPilot.ViewModels;

namespace HostPilot.Dialogs;

public partial class ImportDiscoveredServersDialog : Window
{
    private readonly ImportDiscoveredServersViewModel _viewModel;

    public ImportDiscoveredServersDialog(ImportDiscoveredServersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void OnImportClicked(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedCount == 0)
        {
            MessageBox.Show(
                this,
                "Select at least one discovered server to import, or choose Skip Import to continue without onboarding existing installs.",
                "No Servers Selected",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        DialogResult = true;
        Close();
    }
}
