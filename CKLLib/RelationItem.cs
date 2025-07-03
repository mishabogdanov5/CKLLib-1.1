using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CKLLib
{
    public class RelationItem : ICloneable // Элемент динамического отношения
    {
        public Pair Value { get; set; } // Элемент из множества, задающего отношение

        public List<TimeInterval> Intervals { get; set; } // Интервалы истинности
                                                          // индикаторной функции элемента

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Info { get; set; }

        public RelationItem()
        {
            Value = new Pair([string.Empty, string.Empty]);
            Intervals = new List<TimeInterval>();
        }

        public RelationItem(Pair value, [Required] IEnumerable<TimeInterval> intervals)
        {
            Value = value;

            Intervals = intervals.OrderBy(x => x, new TimeIntervalsComparer()).ToList();
            if (Intervals.Count > 1) Intervals.RemoveAll(x => x.Equals(TimeInterval.ZERO));
        }

        public RelationItem(Pair value, List<TimeInterval> intervals, object? info) : this(value, intervals)
        {
            Info = info;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RelationItem) return false;

            RelationItem? item = obj as RelationItem;
            if (item == null) return false;

            return Value.Equals(item.Value) && Intervals.SequenceEqual(item.Intervals,
                new TimeIntervalEqualityComparer());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Intervals);
        }

        public object Clone()
        {
            List<TimeInterval> newIntervals = new List<TimeInterval>();

            foreach (TimeInterval interval in Intervals)
            {
                newIntervals.Add(new TimeInterval(interval.StartTime, interval.EndTime));
            }

            return new RelationItem(Value, newIntervals, Info);
        }

        private class TimeIntervalEqualityComparer : IEqualityComparer<TimeInterval>
        {
            public bool Equals(TimeInterval? x, TimeInterval? y)
            {
                if (x == null) return false;

                return x.Equals(y);
            }

            public int GetHashCode([DisallowNull] TimeInterval obj)
            {
                return obj.GetHashCode();
            }
        }

    }
    public class TimeIntervalsComparer : IComparer<TimeInterval>
    {
        public int Compare(TimeInterval? x, TimeInterval? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return x.StartTime.CompareTo(y.StartTime);
        }
    }
}
