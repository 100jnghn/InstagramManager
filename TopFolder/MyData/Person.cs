using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace InstagramManager.MyData {
    public class Person {
        public string Value { get; }
        public string Href { get; }
        public int DateFromToday { get; }

        public Person(string value, string href, int dateFromToday) {
            Value = value;
            Href = href;
            DateFromToday = dateFromToday;
        }
    }
}
