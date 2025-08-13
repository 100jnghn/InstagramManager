using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace InstagramManager.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "Instagram Manager";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "언팔로워 조회",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.MainPage)
            },
            new NavigationViewItem()
            {
                Content = "맞팔로우 조회",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.FollowForFollowPage)
            },
            new NavigationViewItem()
            {
                Content = "사용 방법 안내",
                Icon = new SymbolIcon {Symbol = SymbolRegular.DataHistogram24},
                TargetPageType = typeof(Views.Pages.GuidePage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
