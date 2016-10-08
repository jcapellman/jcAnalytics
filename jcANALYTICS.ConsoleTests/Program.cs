using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private static string print(Users user) =>
            $"Has Android? {user.HasAndroid.Value} - Has iPhone? {user.HasIOS.Value} - Has Windows Phone? {user.HasWinPhone.Value} - Lives in Maryland? {user.LivesInMaryland.Value}";

        private static void predictionTest(List<Users> users) {
            var engine = new jcAEngine<Users>();

            var now = DateTime.Now;

            engine.AnalyzeData(users);

            Console.WriteLine(now.Subtract(DateTime.Now).TotalSeconds);

            var mostCommon = engine.GetMostCommon();
            var leastCommon = engine.GetLeastCommon();

            Console.WriteLine($"Most Common: {print(mostCommon.obj)} Percentage {mostCommon.Percentage}");
            Console.WriteLine($"Least Common: {print(leastCommon.obj)} Percentage {leastCommon.Percentage}");

            var tmpItem = new Users { LivesInMaryland = true, HasWinPhone = true };

            var completeItem = engine.GetCompleteItem(tmpItem);

            Console.WriteLine($"Complete Object: {print(completeItem)}");

            using (var sw = new StreamWriter("test.txt")) {
                var items = engine.GetGroupItems();

                foreach (var item in items) {
                    sw.WriteLine(print(item.obj) + " " + item.Count);
                }
            }
        }

        private static void reductionTest(List<Users> users) {
            var now = DateTime.Now;

            var reduced = users.ReduceParallel();

            var parallelTime = DateTime.Now.Subtract(now).TotalSeconds;

            Console.WriteLine($"Reduced from {users.Count()} to {reduced.Count()}");

            now = DateTime.Now;

            reduced = users.Reduce();

            var singleTime = DateTime.Now.Subtract(now).TotalSeconds;

            now = DateTime.Now;

            reduced = users.ReduceAuto();

            var autoTime = DateTime.Now.Subtract(now).TotalSeconds;

            now = DateTime.Now;

            reduced = users.ReduceParallelOptimized(false);

            var optTime = DateTime.Now.Subtract(now).TotalSeconds;

            Console.WriteLine("Parallel\tSingle\t\tAuto\t\tOptimized");
            Console.WriteLine($"{parallelTime}\t{singleTime}\t{autoTime}\t{optTime}\n");

            Console.WriteLine($"Reduced from {users.Count()} to {reduced.Count()}");
        }

        static void Main(string[] args) {
            int testCaseSize = 50;

            if (args.Length > 0) {
                testCaseSize = Convert.ToInt32(args[0]);
            }

            var users = generateData(testCaseSize);

            Console.WriteLine("1)Prediction Test");
            Console.WriteLine("2)Reduction Test");

            switch (Console.ReadLine()) {
                case "1":
                    predictionTest(users);
                    break;
                case "2":
                    reductionTest(users);
                    break;
            }

            Console.ReadKey();
        }
    }
}