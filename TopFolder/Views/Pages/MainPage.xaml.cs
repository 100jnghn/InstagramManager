using InstagramManager.ViewModels.Pages;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Navigation;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.Views.Pages
{
    public partial class MainPage : INavigableView<MainViewModel>
    {
        public MainViewModel ViewModel { get; }

        public MainPage(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            InitializeComponent();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "IsFileUploaded":
                    this.btnSearch.Visibility = Visibility.Visible;

                    break;

                case "IsSearchFinished":
                    this.btnExelUnfollower.Visibility = Visibility.Visible;
                    this.btnExelRecentlyUnfollower.Visibility = Visibility.Visible;
                    this.btnSaveF4F.Visibility = Visibility.Visible;

                    break;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
