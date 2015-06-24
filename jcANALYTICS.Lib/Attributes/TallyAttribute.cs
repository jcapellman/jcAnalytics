using System;

namespace jcANALYTICS.Lib.Attributes {
    public class TallyAttribute : Attribute {
        public string Name;

        public TallyAttribute() { }

        public TallyAttribute(string name) { Name = name; }
    }
}