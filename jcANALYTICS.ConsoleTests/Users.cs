using System;

using jcANALYTICS.Lib.Attributes;
using jcANALYTICS.Lib.Transports;

namespace jcANALYTICS.ConsoleTests {
    [Serializable]
    public class Users : jcAnalyticsBaseObject {
        [Tally]("Has iOS")
        public bool? HasIOS { get; set; }

        [Tally]
        public bool? HasAndroid { get; set; }

        [Tally]
        public bool? HasWinPhone { get; set; }

        [Tally]
        public bool? LivesInMaryland { get; set; }
    }
}