using InstagramManager.ViewModels.Pages;
using System.Diagnostics;
using System.Windows.Navigation;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.Views.Pages
{
    public partial class FollowForFollowPage : INavigableView<FollowForFollowViewModel>
    {
        public FollowForFollowViewModel ViewModel { get; }

        public FollowForFollowPage(FollowForFollowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
