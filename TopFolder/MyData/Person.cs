using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace InstagramManager.MyData {
    public class Person {
        private string value;
        private string href;
        private int dateFromToday;

        public Person(string value, string href, int dateFromToday) {
            this.value = value;
            this.href = href;
            this.dateFromToday = dateFromToday;
        }

        public string getValue() {
            return this.value;
        }

        public string getHref() {
            return this.href;
        }

        public int getDateFromToday() {
            return this.dateFromToday;
        }
    }
}
