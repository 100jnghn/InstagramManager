using InstagramManager.Interfaces;
using InstagramManager.Models;
using System.CodeDom;

namespace InstagramManager.Services
{
    class FollowForFollowService : IDatabase<FollowForFollowTable> {

        private readonly InstagramManagerDatabaseContext dbContext;

        public FollowForFollowService(InstagramManagerDatabaseContext dbContext) {
            this.dbContext = dbContext;
        }

        public void DeleteData(string id) {

            this.dbContext.SaveChanges();
        }

        // 맞팔 Table 모든 data 반환
        public List<FollowForFollowTable> GetAllData() {
            return this.dbContext.FollowForFollowTables.ToList();
        }

        // 맞팔 table 특정 data 반환
        public FollowForFollowTable? GetData(string id) {

            // 일치하는 id에 해당하는 data를 반환
            // 일치하는 id가 없으면 null을 반환
            return this.dbContext.FollowForFollowTables.FirstOrDefault(c => c.Id == id);
        }

        // 특정 entity 삽입
        public void InsertData(FollowForFollowTable entity) {
            this.dbContext.FollowForFollowTables.Add(entity);
            this.dbContext.SaveChanges();
        }

        public void UpdateData(FollowForFollowTable entity) {
            this.dbContext.FollowForFollowTables.Update(entity);
            this.dbContext.SaveChanges();
        }
    }
}
