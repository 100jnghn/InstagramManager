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

        // 맞팔 목록 반환
        // 맞팔 날짜는 follower.DateFromToday / following.DateFromToday 중 작은 값
        public IEnumerable<Person> GetF4F() {
            return followings
                .Where(kvp => followers.ContainsKey(kvp.Key))
                .Select(kvp => {

                    var id = kvp.Key;
                    var following = kvp.Value;
                    var follower = followers[id];

                    var date = Math.Min(following.DateFromToday, follower.DateFromToday);

                    return new Person(following.Value, following.Href, date);
                });
        }

        // 최근 팔로우 끊긴 계정들 반환
        public IEnumerable<Person> GetRecentlyUnfollowed() {
            return recentlyUnfollowed.Values;
        }
    }
}
