namespace jcANALYTICS.Lib.Transports.Internal {
    public class jcAnalyticsGroupItem<T> where T : jcAnalyticsBaseObject {
        public int Count { get; set; }

        public T obj { get; set; }
    }
}