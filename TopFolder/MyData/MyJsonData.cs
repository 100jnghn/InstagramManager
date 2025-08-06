using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramManager.MyData {
    internal class MyJsonData {

        private Dictionary<string, Person> followers = new Dictionary<string, Person>();                // 내 팔로워
        private Dictionary<string, Person> followings = new Dictionary<string, Person>();               // 내 팔로잉
        private Dictionary<string, Person> recentlyUnfollowed = new Dictionary<string, Person>();       // 최근 언팔 목록



        public void addFollower(string value, Person person) {
            followers.Add(value, person);
        }

        public void addFollowing(string value, Person person) {
            followings.Add(value, person);
        }

        public void addRecentlyUnfollowed(string value, Person person) {
            recentlyUnfollowed.Add(value, person);
        }

        // followers에는 없지만 following에는 있는
        // 사람 찾기
        // 언팔 목록 찾아서 반환
        public IEnumerable<Person> GetUnfollowers() {
            return followings
                .Where(kvp => !followers.ContainsKey(kvp.Key))
                .Select(kvp => kvp.Value);
        }

        // 최근 팔로우 끊긴 계정들 반환
        public IEnumerable<Person> GetRecentlyUnfollowed() {
            return recentlyUnfollowed.Values;
        }
    }
}
