using System.Windows.Controls;
using SteamServerTool.ViewModels;

namespace SteamServerTool.Controls;

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
