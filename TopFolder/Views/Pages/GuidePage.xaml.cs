using InstagramManager.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.Views.Pages
{
    public partial class GuidePage : INavigableView<GuideViewModel> {

        public GuideViewModel ViewModel { get; }

        public GuidePage(GuideViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
