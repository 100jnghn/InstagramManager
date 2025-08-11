using System.Windows.Media;
using InstagramManager.Interfaces;
using InstagramManager.Models;
using InstagramManager.MyData;
using RBush;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private readonly IDatabase<FollowForFollow> database;
        private bool _isInitialized = false;

        [ObservableProperty]
        private IEnumerable<Person> f4f;

        [ObservableProperty]
        private bool isF4FUpdated;
        
        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
            
        private void InitializeViewModel()
        {
            
            _isInitialized = true;
        }
    }
}
