using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace jcANALYTICS.Lib {
    public class jcReduce<T> {
        public jcReduce() { }

        public List<T> Reduce(List<T> originalSet) {
            var reduced = ImmutableDictionary<int, T>.Empty;

            foreach (var item in originalSet) {
                var hash = item.GetHashCode();

                if (reduced.ContainsKey(hash)) {
                    continue;
                }

                reduced = reduced.Add(hash, item);
            }

            return reduced.Values.ToList();
        }

        public List<T> ReduceParallel(List<T> originalSet) {
            var reduced = ImmutableDictionary<int, T>.Empty;
            
            Parallel.ForEach(originalSet, item => {
                var hash = item.GetHashCode();

                if (reduced.ContainsKey(hash)) {
                    return;
                }

                reduced = reduced.Add(hash, item);
            });

            return reduced.Values.ToList();
        }         
    }
}