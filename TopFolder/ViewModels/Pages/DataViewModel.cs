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

        // Dashboard에서 맞팔 정보를 생성하면 호출하는 함수
        public void MakeDatabase() {

            // db가 없는 경우 -> table에 f4f 전부 할당
            if (database.GetAllData().Count == 0) {

                foreach (var person in F4f) {
                    var entity = new FollowForFollowTable
                    {
                        Id = person.Value,
                        Address = person.Href,
                        Date = person.DateFromToday
                    };

                    database.InsertData(entity);
                }
            }

            // db가 있는 경우 -> table 데이터 지우고 f4f 할당
            else if (database.GetAllData().Count > 0) {
                // 값 비교?
            }
        }

    }
}
