namespace jcANALYTICS.Lib.Transports.Results {
    public class jcAnalyticsMaxResultItem<T> where T : jcAnalyticsBaseObject {
        public int Count { get; set; }

        public T obj { get; set; }

        public double Percentage { get; set; }
    }
}