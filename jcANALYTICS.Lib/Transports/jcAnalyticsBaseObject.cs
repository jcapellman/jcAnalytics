using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using jcANALYTICS.Lib.Attributes;

namespace jcANALYTICS.Lib.Transports {
    [Serializable]
    public class jcAnalyticsBaseObject {
        public override bool Equals(object obj) {
            var tmp = (jcAnalyticsBaseObject)obj;

            var fields = tmp.GetType().GetProperties();

            foreach (var field in fields.Where(a => a.GetCustomAttribute(typeof(TallyAttribute)) != null)) {
                var originalValue = field.GetValue(this, null);
                var newValue = field.GetValue(tmp, null);

                if (newValue.ToString() != originalValue.ToString()) {
                    return false;
                }

            }

            return true;
        }

        public string GetAttributeName<T>(string property) {
            var props = typeof(T).GetProperties();

            foreach (var prop in props) {
                var attrs = prop.GetCustomAttributes(true);

                foreach (var attr in attrs) {
                    var authAttr = attr as TallyAttribute;

                    if (authAttr != null) {
                        return authAttr.Name;
                    }
                }
            }

            return null;
        }

        public override int GetHashCode() {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            var tmp = ms.ToArray();

            return computeHash(tmp);
        }

        private static int computeHash(params byte[] data) {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}
