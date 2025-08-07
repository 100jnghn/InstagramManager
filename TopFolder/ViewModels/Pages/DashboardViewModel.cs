using Microsoft.Win32;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InstagramManager.MyData;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;
using ClosedXML.Excel;

namespace InstagramManager.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        #region PROPERTY

        [ObservableProperty]
        private string uploadMessage = "Upload File을 누르고 파일을 업로드해주세요";

        [ObservableProperty]
        private bool isFileUploaded = false;

        [ObservableProperty]
        private bool isSearchFinished = false;

        [ObservableProperty]
        private FileInfo uploadedFile;

        [ObservableProperty]
        private IEnumerable<Person> unfollowers;

        [ObservableProperty]
        private IEnumerable<Person> recentlyUnfollowers;

        #endregion

        #region JSON

        private JArray followers_1;
        private JObject following;
        private JObject recently_unfollowed_profiles;

        #endregion

        #region DATA

        private MyJsonData myJsonData;

        #endregion

        #region CONSTRUCTOR

        public DashboardViewModel() {
            
        }

        #endregion

        #region COMMAND

        [RelayCommand]
        private void exportExcelUnfollower() {

            if (Unfollowers == null || !Unfollowers.Any()) {
                // 내보낼 언팔로워가 없습니다.
                return;
            }

            try {
                using (var workbook = new ClosedXML.Excel.XLWorkbook()) {

                    var worksheet = workbook.Worksheets.Add("언팔로워");

                    worksheet.Cell(1, 1).Value = "아이디";
                    worksheet.Cell(1, 2).Value = "주소";
                    worksheet.Cell(1, 3).Value = "팔로우한 일수";



                    int row = 2;

                    foreach (var item in Unfollowers) {
                        worksheet.Cell(row, 1).Value = item.Value;
                        worksheet.Cell(row, 2).Value = item.Href;
                        worksheet.Cell(row, 3).Value = item.DateFromToday;

                        row++;
                    }

                    // 열 너비 지정
                    worksheet.Column(1).AdjustToContents();
                    worksheet.Column(2).AdjustToContents();
                    worksheet.Column(3).Width = 15;

                    // 저장 경로 (로컬저장소 -> 다운로드)
                    string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    string filePath = Path.Combine(downloadPath, "Unfollowers.xlsx");

                    // 저장
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex) {
                // 에러 메시지
                return;
            }
        }

        [RelayCommand]  // 파일 업로드 버튼 클릭
        private void btnFileUploadClicked() {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "ZIP files (*.zip)|*.zip"
            };

            // 파일 불러오기 실패 시 아무 동작 X
            if (openFileDialog.ShowDialog() == false) {
                return;
            }

            // 파일 불러오기 성공 flag
            IsFileUploaded = true;

            // 업로드한 파일을 변수로 저장
            UploadedFile = new FileInfo(openFileDialog.FileName);

            // 업로드한 파일 이름 출력
            this.UploadMessage = "Search Start를 누르고 팔로워를 조회해주세요";
        }

        [RelayCommand]  // 파일 탐색 시작
        private void btnSearchStart() {

            if (UploadedFile == null) {
                this.UploadMessage = "파일이 업로드 되지 않았습니다.";
                return;
            }

            // 인스타 파일이 아닌 경우 종료
            if(!UploadedFile.Name.StartsWith("instagram-") || !UploadedFile.Name.EndsWith(".zip")) {
                this.UploadMessage = "올바른 파일 형식이 아닙니다.";
                return;
            }

            // 압축 해제 위치 (임시 폴더 사용)  // 경로 -> User/Appdata/Temp
            string extractTempPath = Path.Combine(Path.GetTempPath(), "TempPath");

            // 이미 임시 폴더가 존재하는 경우 지우고 새로 생성
            if(File.Exists(extractTempPath)) {
                Directory.Delete(extractTempPath, true);
            }

            // 임시 폴더 생성
            Directory.CreateDirectory(extractTempPath);

            this.UploadMessage = "파일 탐색 중...";

            // 파일 분류
            try {
                categorizeFiles(extractTempPath);
                UploadMessage = "파일 분류 성공";
            }
            catch (Exception ex) {
                UploadMessage = $"파일 분류 중 오류 발생: {ex.Message}";
                return;
            }

            myJsonData = new MyJsonData();

            // JSON 데이터 파싱 
            try {
                parsingFollowerData();
                parsingFollowingData();
                parsingRecentlyUnfollowData();

                UploadMessage = "데이터 조회 성공";
            }
            catch (Exception ex) {
                UploadMessage = $"데이터 분류 중 오류 발생: {ex.Message}";
                return;
            }

            // 나를 팔로우 하지 않는 사람들의 데이터 가져오기
            Unfollowers = myJsonData.GetUnfollowers();

            // 최근 팔로우 끊긴 계정 데이터 가져오기
            RecentlyUnfollowers = myJsonData.GetRecentlyUnfollowed();

            // 파일 탐색 성공 플래그
            IsSearchFinished = true;
        }

        // 압축파일 내부 경로에서 파일을 찾아서 분류 후 변수에 할당
        private void categorizeFiles(string extractTempPath) {

            // 1. 파일 압축 해제
            try {
                ZipFile.ExtractToDirectory(UploadedFile.FullName, extractTempPath, overwriteFiles: true);
            }
            catch (Exception ex) {
                UploadMessage = $"zip파일 압축 실패: {ex.Message}";
                return;
            }



            // 2. 하위 폴더에서 connections 파일 탐색
            string[] connectionsDirs = Directory.GetDirectories(extractTempPath, "connections", SearchOption.AllDirectories);

            // connections 폴더를 찾을 수 없는 경우
            if (connectionsDirs.Length == 0) {
                this.UploadMessage = "connections 폴더를 찾을 수 없습니다.";
                return;
            }

            // connections 폴더 경로
            string connectionsPath = connectionsDirs[0];



            // 3. connections 폴더에서 followers_and_following 폴더 찾기
            string[] followDirs = Directory.GetDirectories(connectionsPath, "followers_and_following", SearchOption.AllDirectories);

            // followers_and_following 폴더를 찾을 수 없는 경우
            if (followDirs.Length == 0) {
                this.UploadMessage = "followers_and_following 폴더를 찾을 수 없습니다.";
                return;
            }

            // followers_and_following 폴더 경로
            string followPath = followDirs[0];



            //  4. followers_and_following 폴더에서 필요한 JSON 파일들 가져오기
            string path;            // 파일 경로
            string jsonContent;     // 파일 내용

            // 4-A followers_1.json 파일 가져오기
            path = Directory.GetFiles(followPath, "followers_1.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
            jsonContent = File.ReadAllText(path);
            followers_1 = JArray.Parse(jsonContent);
            
            // 4-B following.json 파일 가져오기
            path = Directory.GetFiles(followPath, "following.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
            jsonContent = File.ReadAllText(path);
            following = JObject.Parse(jsonContent);

            // 4-C recently_unfollowed_profiles.json 파일 가져오기
            path = Directory.GetFiles(followPath, "recently_unfollowed_profiles.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
            jsonContent = File.ReadAllText(path);
            recently_unfollowed_profiles = JObject.Parse(jsonContent);
        }

        // 내 팔로워 목록 파싱
        private void parsingFollowerData() {

            if(followers_1 == null) {
                UploadMessage = "followers_1.json 파일이 존재하지 않습니다.";
                return;
            }

            // follwer_1.json 파일 순회하며 데이터 Parsing
            foreach (JObject data in followers_1) {

                // 데이터 하나 꺼내기
                JArray stringList = (JArray)data["string_list_data"];

                if (stringList == null) continue;



                // 가져온 데이터 JObject로 변환
                JObject person = (JObject)stringList[0];

                // 프로필링크, 아이디 추출
                string href = person["href"].ToString();
                string value = person["value"].ToString();



                // 타임스탬프 추출
                string timestamp = person["timestamp"].ToString();
                long time = long.Parse(timestamp);

                // 타임스탬프 DataTime으로 변환
                DateTime targetData = DateTimeOffset
                    .FromUnixTimeSeconds(time)
                    .ToOffset(TimeSpan.FromHours(9))
                    .DateTime;

                DateTime today = DateTime.Today.Date;
                int dateFromToday = (today-targetData.Date).Days;



                // 팔로워 객체 생성
                Person follower = new Person(value, href, dateFromToday);

                myJsonData.addFollower(value, follower);
            }
        }

        // 팔로잉 목록 파싱
        private void parsingFollowingData() {

            // following.json에서 relationships_following배열 가져오기
            JArray followingArr = (JArray)following["relationships_following"];

            // follwing.json 파일 순회
            foreach (JObject data in followingArr) {

                // 배열에서 데이터 하나 꺼내기
                JArray stringList = (JArray)data["string_list_data"];

                if (stringList == null) continue;



                // 가져온 데이터 JObject로 변환
                JObject person = (JObject)stringList[0];

                // 프로필링크, 아이디 추출
                string href = person["href"].ToString();
                string value = person["value"].ToString();



                // 타임스탬프 추출
                string timestamp = person["timestamp"].ToString();
                long time = long.Parse(timestamp);

                // 타임스탬프 DataTime으로 변환
                DateTime targetData = DateTimeOffset
                    .FromUnixTimeSeconds(time)
                    .ToOffset(TimeSpan.FromHours(9))
                    .DateTime;

                DateTime today = DateTime.Today.Date;
                int dateFromToday = (today - targetData.Date).Days;



                // 팔로잉 객체 생성
                Person following = new Person(value, href, dateFromToday);

                myJsonData.addFollowing(value, following);
            }
        }

        // 최근 언팔 목록 파싱
        private void parsingRecentlyUnfollowData() {
            // following.json에서 relationships_following배열 가져오기
            JArray recentlyUnfollowArr = (JArray)recently_unfollowed_profiles["relationships_unfollowed_users"];

            // follwing.json 파일 순회
            foreach (JObject data in recentlyUnfollowArr) {

                // 배열에서 데이터 하나 꺼내기
                JArray stringList = (JArray)data["string_list_data"];

                if (stringList == null) continue;



                // 가져온 데이터 JObject로 변환
                JObject person = (JObject)stringList[0];

                // 프로필링크, 아이디 추출
                string href = person["href"].ToString();
                string value = person["value"].ToString();



                // 타임스탬프 추출
                string timestamp = person["timestamp"].ToString();
                long time = long.Parse(timestamp);

                // 타임스탬프 DataTime으로 변환
                DateTime targetData = DateTimeOffset
                    .FromUnixTimeSeconds(time)
                    .ToOffset(TimeSpan.FromHours(9))
                    .DateTime;

                DateTime today = DateTime.Today.Date;
                int dateFromToday = (today - targetData.Date).Days;



                // 팔로잉 객체 생성
                Person recentlyUnfollow = new Person(value, href, dateFromToday);

                myJsonData.addRecentlyUnfollowed(value, recentlyUnfollow);
            }
        }

        #endregion
    }
}
