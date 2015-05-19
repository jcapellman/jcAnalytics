using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace jcANALYTICS.Lib {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class jcReduceExtensions {
        private const int ParallelThreshold = 50000;

        public static List<T> ReduceAuto<T>(this List<T> originalSet) {
            return (originalSet.Count() > ParallelThreshold ? originalSet.ReduceParallel() : originalSet.Reduce());
        }

        public static List<T> Reduce<T>(this List<T> originalSet) {
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

        public static List<T> ReduceParallel<T>(this List<T> originalSet) {
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