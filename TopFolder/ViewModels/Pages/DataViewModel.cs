using DocumentFormat.OpenXml.Presentation;
using InstagramManager.Interfaces;
using InstagramManager.Models;
using InstagramManager.MyData;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Reflection.Metadata;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly IDatabase<FollowForFollowTable> database;

        #region PROPERTY

        [ObservableProperty]
        private IEnumerable<Person> f4f;

        [ObservableProperty]
        private ObservableCollection<FollowForFollowTable> f4fDatabase;

        [ObservableProperty]
        private List<string> id;

        [ObservableProperty]
        private bool isF4FUpdated;

        #endregion

        #region CONSTRUCTOR

        // 생성자에서 db 초기화
        public DataViewModel(IDatabase<FollowForFollowTable> database) {
            this.database = database;
            this.F4fDatabase = new ObservableCollection<FollowForFollowTable>();
        }
        #endregion

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
            
        private void InitializeViewModel()
        {
            F4fDatabase.Clear();

            foreach (var item in database.GetAllData()) {
                F4fDatabase.Add(item);
            }
        }

        #region METHOD

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

                // db 데이터 가져오기
                var data = database.GetAllData();

                // LinQ를 사용해 f4f, db의 ID를 Hashset으로 가져오기
                var dbID = data.Select(f => f.Id).ToHashSet();
                var f4fID = F4f.Select(f=>f.Value).ToHashSet();


                // Q. 아이디를 바꾼 경우는 어떡함???
                // 1. F4f에 있고 database에 있음 -> Update
                var updateId = dbID.Intersect(f4fID);

                foreach(var id in updateId) {

                    // database에서 일치하는 ID 찾기
                    var entity = data.FirstOrDefault(d=>d.Id == id);

                    // F4F에서 일치하는 ID 찾기
                    var updateEntity = F4f.FirstOrDefault(f=>f.Value == id);

                    // 변경사항 반영
                    entity.Id = updateEntity!.Value;
                    entity.Address = updateEntity.Href;
                    entity.Date = updateEntity.DateFromToday;

                    database.UpdateData(entity);
                }

                // 2. F4f에 있고 database에 없음 -> Insert
                var insertId = f4fID.Except(dbID);

                foreach(var id in insertId) {

                    // F4F에서 일치하는 ID 찾기
                    var updateEntity = F4f.FirstOrDefault(f => f.Value == id);

                    var entity = new FollowForFollowTable
                    {
                        Id = updateEntity!.Value,
                        Address = updateEntity.Href,
                        Date = updateEntity.DateFromToday,
                    };

                    database.InsertData(entity);
                }

                // 3. F4f에 없고 database에 있음 -> Delete
                var deleteId = dbID.Except(f4fID);

                foreach(var id in deleteId) {
                    database.DeleteData(id);
                }
            }

            // db 만든 후 id만 리스트에 저장
            MakeIDList();

            // 뷰에 사용할 f4f데이터 변경
            F4fDatabase.Clear();

            foreach(var entity in database.GetAllData()) {
                F4fDatabase.Add(entity);
            }
        }

        // DB에서 맞팔로우 아이디를 id에 저장
        private void MakeIDList() {
            Id = F4f.Select(p => p.Value).ToList();
        }

        #endregion

        #region COMMAND
        
        #endregion
    }
}
