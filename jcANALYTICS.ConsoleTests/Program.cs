using System;
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

        private static void predictionTest(List<Users> users) {
            var engine = new jcAEngine<Users>();

            var now = DateTime.Now;

            engine.AnalyzeData(users);

            Console.WriteLine(now.Subtract(DateTime.Now).TotalSeconds);

            var mostCommon = engine.GetMostCommon();
            var leastCommon = engine.GetLeastCommon();

            Console.WriteLine(String.Format("Most Common: {0} Percentage {1}", print(mostCommon.obj), mostCommon.Percentage));
            Console.WriteLine(String.Format("Least Common: {0} Percentage {1}", print(leastCommon.obj), leastCommon.Percentage));

            var tmpItem = new Users { LivesInMaryland = true, HasWinPhone = true };

            var completeItem = engine.GetCompleteItem(tmpItem);

            Console.WriteLine(String.Format("Complete Object: {0}", print(completeItem)));

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

            now  = DateTime.Now;

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
            var users = generateData(Convert.ToInt32(args[0]));

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
