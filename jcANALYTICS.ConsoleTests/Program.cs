﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jcANALYTICS.Lib;

namespace jcANALYTICS.ConsoleTests {
    class Program {
        private static List<Users> generateData(int count) {
            var users = new List<Users>();

            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (var x = 0; x < count; x++) {
                users.Add(new Users {
                    HasIOS = rnd.Next(0, 1) == 1,
                    HasAndroid = rnd.Next(0, 104) % 2 == 0,
                    HasWinPhone = rnd.Next(0, 25) % 2 == 0,
                    LivesInMaryland = rnd.Next(0, 100) % 2 == 0
                });
            }

            return users;
        }

        private static string print(Users user) { return String.Format("Has Android? {0} - Has iPhone? {1} - Has Windows Phone? {2} - Lives in Maryland? {3}", user.HasAndroid.Value, user.HasIOS.Value, user.HasWinPhone.Value, user.LivesInMaryland.Value); }

        static void Main(string[] args) {
            var users = generateData(Convert.ToInt32(args[0]));

            var engine = new jcAEngine<Users>();

            var now = DateTime.Now;

            engine.AnalyzeData(users);

            Console.WriteLine(now.Subtract(DateTime.Now).TotalSeconds);

            var mostCommon = engine.GetMostCommon();
            var leastCommon = engine.GetLeastCommon();

            Console.WriteLine(String.Format("Most Common: {0} Percentage {1}", print(mostCommon.obj), mostCommon.Percentage));
            Console.WriteLine(String.Format("Least Common: {0} Percentage {1}", print(leastCommon.obj), leastCommon.Percentage));

            var tmpItem = new Users {LivesInMaryland = true, HasWinPhone = true};

            var completeItem = engine.GetCompleteItem(tmpItem);

            Console.WriteLine(String.Format("Complete Object: {0}", print(completeItem)));

            using (var sw = new StreamWriter("test.txt")) {
                var items = engine.GetGroupItems();

                foreach (var item in items) {
                    sw.WriteLine(print(item.obj) + " " + item.Count);
                }
            }

            Console.ReadKey();
        }
    }
}