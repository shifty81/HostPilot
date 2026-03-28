using System.Windows.Controls;
using HostPilot.ViewModels;

namespace HostPilot.Controls;

public partial class ModsTabView : UserControl
{
    public ModsTabView()
    {
        InitializeComponent();
    }

    public ModsTabViewModel? ViewModel
    {
        get => DataContext as ModsTabViewModel;
        set => DataContext = value;
    }
}
