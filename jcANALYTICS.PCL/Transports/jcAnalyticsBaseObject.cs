using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jcANALYTICS.PCL.Attributes;

namespace jcANALYTICS.PCL.Transports {
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
