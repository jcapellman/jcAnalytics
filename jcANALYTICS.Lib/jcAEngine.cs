﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using jcANALYTICS.Lib.Attributes;
using jcANALYTICS.Lib.Transports;
using jcANALYTICS.Lib.Transports.Internal;
using jcANALYTICS.Lib.Transports.Results;

namespace jcANALYTICS.Lib {
    public class jcAEngine<T> where T : jcAnalyticsBaseObject {
        private ImmutableDictionary<int, int> _mainStore = ImmutableDictionary<int, int>.Empty;
        private ImmutableList<jcAnalyticsGroupItem<T>> _storeValues = ImmutableList<jcAnalyticsGroupItem<T>>.Empty;

        private int _originalDataCount;
        private readonly bool _enableMT;

        public jcAEngine(bool enableMT = true) {
            _enableMT = enableMT;
        }

        public void AnalyzeData(List<T> data) {
            _originalDataCount = data.Count;

            if (_originalDataCount > 1000 && _enableMT) {
                analyzeDataMT(data);
            } else {
                analyzeDataST(data);
            }
        }

        private void analyzeDataMT(List<T> data) {
            Parallel.ForEach(data, processItem);
        }

        private void analyzeDataST(List<T> data) {
            foreach (var item in data) {
                processItem(item);
            }
        }

        private void processItem(T item) {
            var hashCode = item.GetHashCode();

            if (_mainStore.ContainsKey(hashCode)) {
                var al = _mainStore.GetValueOrDefault(hashCode);

                _mainStore = _mainStore.SetItem(hashCode, ++al);

                foreach (var subItem in _storeValues.Where(subItem => subItem.obj.Equals(item))) {
                    subItem.Count++;
                    break;
                }
            } else {
                _mainStore = _mainStore.SetItem(hashCode, 1);

                _storeValues = _storeValues.Add(new jcAnalyticsGroupItem<T> { Count = 1, obj = item });
            }
        }

        public jcAnalyticsMaxResultItem<T> GetMostCommon() {
            var val = _storeValues.OrderByDescending(a => a.Count).FirstOrDefault();

            return new jcAnalyticsMaxResultItem<T> { Count = val.Count, obj = val.obj, Percentage = 100 * ((double)val.Count / _originalDataCount) };
        }

        public jcAnalyticsMinResultItem<T> GetLeastCommon() {
            var val = _storeValues.OrderBy(a => a.Count).FirstOrDefault();

            return new jcAnalyticsMinResultItem<T> { Count = val.Count, obj = val.obj, Percentage = 100 * ((double)val.Count / _originalDataCount) };
        }

        public List<jcAnalyticsGroupItem<T>> GetGroupItems() { return _storeValues.ToList(); }

        private T getCloseItems(T incompleteItem) {
            T tmpItem = null;

            var maxCount = 0;

            foreach (var item in _storeValues) {
                var fields = incompleteItem.GetType().GetProperties();

                var okToAdd = true;

                foreach (var field in fields.Where(a => a.GetCustomAttribute(typeof (TallyAttribute)) != null)) {
                    var originalValue = field.GetValue(incompleteItem, null);

                    if (originalValue == null) {
                        continue;
                    }

                    var newValue = field.GetValue(item.obj, null);

                    if (newValue.ToString() != originalValue.ToString()) {
                        okToAdd = false;
                        break;
                    }
                }

                if (okToAdd && item.Count > maxCount) {
                    tmpItem = item.obj;
                    maxCount = item.Count;
                }
            }

            return tmpItem;
        } 

        public T GetCompleteItem(T incompleteItem) {
            return getCloseItems(incompleteItem);
        }
    }
}