using InstagramManager.Interfaces;
using InstagramManager.Models;
using InstagramManager.MyData;
using Microsoft.EntityFrameworkCore.Storage;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly IDatabase<FollowForFollowTable> database;

        [ObservableProperty]
        private IEnumerable<Person> f4f;

        [ObservableProperty]
        private bool isF4FUpdated;

        // 생성자에서 db 초기화
        public DataViewModel(IDatabase<FollowForFollowTable> database) {
            this.database = database;
        }

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
