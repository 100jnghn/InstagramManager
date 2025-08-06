using Microsoft.Win32;
using System.IO.Compression;
using System.IO;
using System.Reflection;

namespace InstagramManager.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        #region PROPERTY

        [ObservableProperty]
        private string uploadMessage = "파일을 업로드하세요";

        [ObservableProperty]
        private bool isFileUploaded = false;

        [ObservableProperty]
        private FileInfo uploadedFile;

        

        #endregion

        #region CONSTRUCTOR

        public DashboardViewModel() {
            
        }

        #endregion

        #region COMMAND

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
            this.UploadMessage = UploadedFile.Name;
        }

        [RelayCommand]  // 파일 탐색 시작
        private void btnSearchStart() {
            if (UploadedFile == null) {
                return;
            }

            // 압축 해제 위치 (임시 폴더 사용)
            string extractTempPath = Path.Combine(Path.GetTempPath(), "TempPath");

            // 임시 폴더 생성
            Directory.CreateDirectory(extractTempPath);

            this.UploadMessage = "파일 탐색 중...";

            try {
                // 1. 파일 압축 해제
                ZipFile.ExtractToDirectory(UploadedFile.FullName, extractTempPath, overwriteFiles: true);

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
                if(followDirs.Length == 0) {
                    this.UploadMessage = "followers_and_following 폴더를 찾을 수 없습니다.";
                    return;
                }

                // followers_and_following 폴더 경로
                string followPath = followDirs[0];

                //  4. followers_and_following 폴더에서 필요한 JSON 파일들 가져오기
            }
            catch (Exception ex) {
                UploadMessage = $"압축 해제 중 오류 발생: {ex.Message}";
            }
        }




        #endregion
    }
}
