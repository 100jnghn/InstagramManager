using DocumentFormat.OpenXml.Presentation;
using InstagramManager.Interfaces;
using InstagramManager.Models;
using InstagramManager.MyData;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using Wpf.Ui.Abstractions.Controls;

namespace InstagramManager.ViewModels.Pages
{
    public partial class FollowForFollowViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly IDatabase<FollowForFollowTable> database;

        #region PROPERTY

        [ObservableProperty]
        private IEnumerable<Person> f4f;

        [ObservableProperty]
        private ObservableCollection<FollowForFollowTable> f4fDatabase;

        [ObservableProperty]
        private ObservableCollection<string> id;

        [ObservableProperty]
        private bool isF4FUpdated;

        [ObservableProperty]
        private FollowForFollowTable? selectedRow;

        [ObservableProperty]
        private string selectedID;

        [ObservableProperty]
        private string selectedAddress;

        [ObservableProperty]
        private int selectedDate;

        [ObservableProperty]
        private string? selectedDescription;

        #endregion

        #region CONSTRUCTOR

        // 생성자에서 db 초기화
        public FollowForFollowViewModel(IDatabase<FollowForFollowTable> database) {
            this.database = database;
            this.F4fDatabase = new ObservableCollection<FollowForFollowTable>();
            this.Id = new();
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

            MakeIDList();
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
                    // entity.Description은 유지

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



            // 뷰에 사용할 f4f데이터 변경
            LoadDatabase();

            // db 만든 후 id만 리스트에 저장
            MakeIDList();
        }

        // F4fDatabase를 새롭게 reload
        private void LoadDatabase() {
            F4fDatabase.Clear();

            foreach (var entity in database.GetAllData()) {
                F4fDatabase.Add(entity);
            }
        }

        // DB에서 맞팔로우 아이디를 id에 저장
        private void MakeIDList() {
            Id.Clear();

            foreach (var idValue in database.GetAllData().Select(d => d.Id)) {
                Id.Add(idValue);
            }
        }

        partial void OnSelectedRowChanged(FollowForFollowTable? value) {
            if (value == null) return;
            
            SelectedID = value.Id;
            btnSearch();    
        }

        #endregion

        #region COMMAND

        [RelayCommand]
        private void btnSearch() {
            var entity = database.GetData(SelectedID);

            if (entity == null) {
                // 일치하는 id 없음
                return;
            }

            this.SelectedAddress = entity.Address;
            this.SelectedDate = entity.Date;
            this.SelectedDescription = entity.Description;
        }

        [RelayCommand]
        private void btnSaveMemo() {
            try {
                // ID에 해당하는 데이터를 가져옴
                var data = this.database.GetData(this.SelectedID);

                // 가져온 데이터에 현재 입력된 메모 할당
                data.Description = this.SelectedDescription;
                
                // db 업데이트
                this.database.UpdateData(data);

                // 데이터 새로 불러오기
                LoadDatabase();
            }
            catch (Exception) {
                MessageBox.Show($"존재하지 않는 ID입니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        #endregion
    }
}
