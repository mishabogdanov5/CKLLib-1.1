using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CKLLib
{
    namespace Operations
    {
        public static class CKLMath // Статический класс, содержащий
                                    // операции алгебры 
                                    // Создан на подобии класса Math в С#
        {

            private static string GetNewFilePath(string path, string newName)
            {
                string fileDirPath = Path.GetDirectoryName(path);
                return Path.Combine(fileDirPath, newName + Path.GetExtension(path));
            }

            //TimeOperations
            public static CKL TimeTransform(CKL ckl, TimeInterval newInterval)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "time_transform_" + name);

                TimeInterval generalInterval = IntervalConjunction(newInterval, ckl.GlobalInterval);

                if (generalInterval.Equals(TimeInterval.ZERO))
                    return new CKL(newPath, newInterval, ckl.Dimention, ckl.Source, new HashSet<RelationItem>());

                HashSet<RelationItem> items = new HashSet<RelationItem>();
                List<TimeInterval> timeIntervals = new List<TimeInterval>();

                double newSTime;
                double newETime;

                foreach (RelationItem item in ckl.Relation)
                {
                    timeIntervals.Clear();

                    for (int i = 0; i < item.Intervals.Count; i++)
                    {
                        newSTime = item.Intervals[i].StartTime >= generalInterval.StartTime ?
                            item.Intervals[i].StartTime : generalInterval.StartTime;

                        newETime = item.Intervals[i].EndTime >= generalInterval.EndTime ?
                            generalInterval.EndTime : item.Intervals[i].EndTime;

                        if (newSTime < newETime) timeIntervals.Add(
                            new TimeInterval(newSTime, newETime));
                    }

                    if (timeIntervals.Count > 0) items.Add(new RelationItem(item.Value, timeIntervals, item.Info));
                    else items.Add(new RelationItem(item.Value, new List<TimeInterval>()
                    { TimeInterval.ZERO}, item.Info));
                }

                return new CKL(newPath, newInterval, ckl.Dimention, ckl.Source, items);
            }

            public static CKL LeftPrecedence(CKL ckl, TimeInterval interval, double t)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");
                //TODO: interval null

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, $"left_precedence_{t}_" + name);

                if (interval.StartTime - t <= ckl.GlobalInterval.StartTime)
                    return new CKL(newPath, TimeInterval.ZERO, ckl.Dimention, ckl.Source, new HashSet<RelationItem>());

                TimeInterval newInterval = new TimeInterval(ckl.GlobalInterval.StartTime,
                    interval.StartTime - t < ckl.GlobalInterval.EndTime ? interval.StartTime - t : ckl.GlobalInterval.EndTime);


                CKL res = TimeTransform(ckl, newInterval);
                res.FilePath = newPath;

                return res;
            }

            public static CKL RightContinuation(CKL ckl, TimeInterval interval, double t)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, $"right_continuation_{t}_" + name);

                if (interval.EndTime + t >= ckl.GlobalInterval.EndTime)
                    return new CKL(newPath, TimeInterval.ZERO, ckl.Dimention, ckl.Source, new HashSet<RelationItem>());

                TimeInterval newInterval =
                new TimeInterval(
    ckl.GlobalInterval.StartTime > interval.EndTime + t ? ckl.GlobalInterval.StartTime : interval.EndTime + t,
    ckl.GlobalInterval.EndTime);

                CKL res = TimeTransform(ckl, newInterval);
                res.FilePath = newPath;

                return res;
            }

            public static CKL LeftContinuation(CKL ckl, TimeInterval interval, double t)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, $"left_continuation_{t}_" + name);

                if (interval.StartTime + t >= ckl.GlobalInterval.EndTime)
                    return new CKL(newPath, TimeInterval.ZERO, ckl.Dimention, ckl.Source, new HashSet<RelationItem>());

                TimeInterval newInterval =
                new TimeInterval(
    ckl.GlobalInterval.StartTime > interval.StartTime + t ? ckl.GlobalInterval.StartTime : interval.StartTime + t,
    ckl.GlobalInterval.EndTime);

                CKL res = TimeTransform(ckl, newInterval);
                res.FilePath = newPath;

                return res;
            }

            public static CKL RightPrecedence(CKL ckl, TimeInterval interval, double t)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, $"right_precedence_{t}_" + name);

                if (interval.EndTime - t <= ckl.GlobalInterval.StartTime)
                    return new CKL(newPath, TimeInterval.ZERO, ckl.Dimention, ckl.Source, new HashSet<RelationItem>());

                TimeInterval newInterval = new TimeInterval(ckl.GlobalInterval.StartTime,
                    interval.EndTime - t < ckl.GlobalInterval.EndTime ? interval.EndTime - t : ckl.GlobalInterval.EndTime);

                CKL res = TimeTransform(ckl, newInterval);
                res.FilePath = newPath;

                return res;
            }

            //Source operations

            public static CKL SourceConstriction(CKL ckl, Func<Pair, bool> selector)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                HashSet<Pair> newSource = new HashSet<Pair>();
                HashSet<RelationItem> newRelation = new HashSet<RelationItem>();

                foreach (RelationItem item in ckl.Relation)
                {
                    if (selector(item.Value))
                    {
                        newSource.Add(item.Value);
                        newRelation.Add((RelationItem)item.Clone());
                    }
                }

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "constriction_" + name);

                return new CKL(newPath, ckl.GlobalInterval, ckl.Dimention, newSource, newRelation);
            }

            public static CKL SourceExpansion(CKL ckl, IEnumerable<Pair> expansion)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                HashSet<Pair> newSource = ckl.Source.Concat(expansion).ToHashSet();

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "expansion_" + name);

                return new CKL(newPath, ckl.GlobalInterval, ckl.Dimention, newSource, ckl.Relation);
            }

            public static bool SourceEquality(HashSet<Pair>? s1, HashSet<Pair>? s2)
            {
                if (s1 == null && s2 == null) return true;

                if (s1 == null) return false;
                if (s2 == null) return false;

                bool areEqual = true;

                foreach (Pair p in s1)
                {
                    if (!s2.Contains(p, new Pair.PairEqualityComparer())) areEqual = false;
                }

                foreach (Pair p in s2)
                {
                    if (!s1.Contains(p, new Pair.PairEqualityComparer())) areEqual = false;
                }

                return areEqual;
            }

            //CKL Logic operations

            private static void TryThrowBinaryExceptions(CKL ckl1, CKL ckl2)
            {
                if (ckl1 == null) throw new ArgumentNullException();
                if (ckl2 == null) throw new ArgumentNullException();

                if (!ckl1.GlobalInterval.Equals(ckl2.GlobalInterval)) throw new ArgumentException();
                if (!ckl1.Dimention.Equals(ckl2.Dimention)) throw new ArgumentException();
                if (!SourceEquality(ckl1.Source, ckl2.Source)) throw new ArgumentException();
            }

            private static TimeInterval IntervalsDisjunction(TimeInterval i1, TimeInterval i2)
            {
                if (i1.StartTime > i2.EndTime || i2.StartTime > i1.EndTime) return TimeInterval.ZERO;

                return new TimeInterval
                    (
                        i1.StartTime > i2.StartTime ? i2.StartTime : i1.StartTime,
                        i1.EndTime > i2.EndTime ? i1.EndTime : i2.EndTime
                    );
            }

            private static bool IsIntervalCombine(TimeInterval i1, TimeInterval i2)
            {
                return (IntervalsDisjunction(i1, i2).Equals(i2));
            }

            private static bool IsIntervalInserted(TimeInterval timeInterval, List<TimeInterval> intervals)
            {
                if (timeInterval.Equals(TimeInterval.ZERO)) return true;

                foreach (TimeInterval interval in intervals)
                {
                    if (IsIntervalCombine(timeInterval, interval)) return true;
                }

                return false;
            }

            private static void InsertInterval(TimeInterval timeInterval, List<TimeInterval> intervals)
            {
                List<TimeInterval> remove = new List<TimeInterval>();
                TimeInterval temp = timeInterval;

                foreach (TimeInterval interval in intervals)
                {
                    TimeInterval disjunction = IntervalsDisjunction(timeInterval, interval);
                    if (!disjunction.Equals(TimeInterval.ZERO))
                    {
                        temp = disjunction;
                        remove.Add(interval);
                    }

                }

                if (remove.Count > 0)
                {
                    intervals.Add(temp);

                    foreach (TimeInterval interval in remove) intervals.Remove(interval);
                }
                else intervals.Add(temp);
            }

            private static List<TimeInterval> IntervalsUnion(List<TimeInterval> intervals1, List<TimeInterval> intervals2)
            {
                List<TimeInterval> res = new List<TimeInterval>();
                TimeInterval temp = TimeInterval.ZERO;

                foreach (TimeInterval interval1 in intervals1)
                {
                    temp = new TimeInterval(interval1.StartTime, interval1.EndTime);
                    foreach (TimeInterval interval2 in intervals2)
                    {
                        TimeInterval disjunction = IntervalsDisjunction(temp, interval2);
                        if (!disjunction.Equals(TimeInterval.ZERO)) temp = disjunction;
                    }

                    if (!IsIntervalInserted(temp, res)) InsertInterval(temp, res);
                }

                foreach (TimeInterval interval in intervals2)
                {
                    if (!IsIntervalInserted(interval, res))
                        res.Add(new TimeInterval(interval.StartTime, interval.EndTime));
                }

                if (res.Count == 0) return new List<TimeInterval>() { TimeInterval.ZERO };

                return res;
            }

            public static List<TimeInterval> OrderedIntervalsUnion(List<TimeInterval> intervals1, List<TimeInterval> intervals2)
            {
                List<TimeInterval> res = IntervalsUnion(intervals1, intervals2);

                if (res.Count > 1) res.RemoveAll(x => x.Equals(TimeInterval.ZERO));
                return res.OrderBy(x => x, new TimeIntervalsComparer()).ToList();
            }

            public static CKL Union(CKL ckl1, CKL ckl2) // объединение динамических отношений
            {
                TryThrowBinaryExceptions(ckl1, ckl2);
                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    foreach (RelationItem item2 in ckl2.Relation)
                    {
                        if (item1.Value.Equals(item2.Value))
                        {
                            relation.Add(new RelationItem(item1.Value,
                                IntervalsUnion(item1.Intervals, item2.Intervals)));
                            break;
                        }

                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "union_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, ckl1.Source, relation);
            }

            private static TimeInterval IntervalConjunction(TimeInterval i1, TimeInterval i2)
            {
                if (i1.StartTime >= i2.EndTime || i2.StartTime >= i1.EndTime)
                    return TimeInterval.ZERO;
                return new TimeInterval
                    (
                        i1.StartTime >= i2.StartTime ? i1.StartTime : i2.StartTime,
                        i1.EndTime >= i2.EndTime ? i2.EndTime : i1.EndTime
                    );
            }

            private static List<TimeInterval> IntervalsIntersection(List<TimeInterval> intervals1, List<TimeInterval> intervals2)
            {
                List<TimeInterval> res = new List<TimeInterval>();

                TimeInterval current = TimeInterval.ZERO;

                foreach (TimeInterval i1 in intervals1)
                {
                    foreach (TimeInterval i2 in intervals2)
                    {
                        current = IntervalConjunction(i1, i2);
                        if (!current.Equals(TimeInterval.ZERO)) res.Add(current);
                    }
                }

                if (res.Count == 0) return new List<TimeInterval> { TimeInterval.ZERO };

                return res;
            }

            public static CKL Intersection(CKL ckl1, CKL ckl2) // пересечение динамических отношений
            {
                TryThrowBinaryExceptions(ckl1, ckl2);
                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    foreach (RelationItem item2 in ckl2.Relation)
                    {
                        if (item1.Value.Equals(item2.Value))
                        {
                            relation.Add(new RelationItem(item1.Value,
                                IntervalsIntersection(item1.Intervals, item2.Intervals)));
                            break;
                        }
                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "intersect_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, ckl1.Source, relation);
            }

            private static bool IsIntervalsEmpty(IEnumerable<TimeInterval> intervals)
            {
                return intervals.SequenceEqual(new List<TimeInterval>() { TimeInterval.ZERO });
            }

            private static List<TimeInterval> IntervalsInversion(List<TimeInterval> intervals, TimeInterval global)
            {
                TimeInterval temp = TimeInterval.ZERO;
                List<TimeInterval> currentIntervals = new List<TimeInterval>();

                if (intervals.Count == 1)
                {
                    if (intervals[0].StartTime > global.StartTime)
                    {
                        temp = new TimeInterval(global.StartTime, intervals[0].StartTime);
                        currentIntervals.Add(temp);
                    }

                    if (intervals[0].EndTime < global.EndTime)
                    {
                        temp = new TimeInterval(intervals[0].EndTime, global.EndTime);
                        currentIntervals.Add(temp);
                    }

                    if (intervals[0].EndTime == global.EndTime &&
                        intervals[0].StartTime == global.StartTime)
                    {
                        return new List<TimeInterval>()
                            { TimeInterval.ZERO };
                    }

                    return currentIntervals;
                }

                for (int i = 0; i < intervals.Count; i++)
                {
                    if (i == 0)
                    {
                        if (intervals[i].StartTime > global.StartTime)
                        {
                            temp = new TimeInterval(global.StartTime, intervals[0].StartTime);
                        }
                        else temp = TimeInterval.ZERO;
                    }

                    if (i == intervals.Count - 1)
                    {
                        temp = new TimeInterval(intervals[i - 1].EndTime, intervals[i].StartTime);

                        if (!temp.Equals(TimeInterval.ZERO)) currentIntervals.Add(temp);

                        if (intervals[i].EndTime < global.EndTime)
                        {
                            temp = new TimeInterval(intervals[i].EndTime, global.EndTime);
                        }
                        else temp = TimeInterval.ZERO;
                    }

                    if (i != 0 && i != intervals.Count - 1)
                    {
                        temp = new TimeInterval(intervals[i - 1].EndTime, intervals[i].StartTime);
                    }

                    if (!temp.Equals(TimeInterval.ZERO))
                        currentIntervals.Add(temp);
                }

                return currentIntervals;
            }

            private static bool HaveSamePoints(TimeInterval timeInterval, List<TimeInterval> intervals)
            {
                foreach (TimeInterval interval in intervals)
                {
                    if (!IntervalConjunction(timeInterval, interval).Equals(TimeInterval.ZERO)) return true;
                }

                return false;
            }

            private static List<TimeInterval> IntervalsDifference(List<TimeInterval> intervals1, List<TimeInterval> intervals2)
            {
                List<TimeInterval> res = new List<TimeInterval>();
                List<TimeInterval> currentDif = new List<TimeInterval>();
                TimeInterval temp = TimeInterval.ZERO;

                foreach (TimeInterval interval1 in intervals1)
                {
                    if (!HaveSamePoints(interval1, intervals2)) res.Add(interval1);
                    else
                    {
                        currentDif.Clear();
                        foreach (TimeInterval interval2 in intervals2)
                        {
                            temp = IntervalConjunction(interval1, interval2);
                            if (!temp.Equals(TimeInterval.ZERO)) currentDif.Add(temp);
                        }
                        res.AddRange(IntervalsInversion(currentDif, interval1));
                    }

                }

                if (res.Count == 0) return new List<TimeInterval>() { TimeInterval.ZERO };
                return res;
            }

            public static CKL Difference(CKL ckl1, CKL ckl2)
            {
                TryThrowBinaryExceptions(ckl1, ckl2);
                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    foreach (RelationItem item2 in ckl2.Relation)
                    {
                        if (item1.Value.Equals(item2.Value))
                        {
                            relation.Add(new RelationItem(item1.Value,
                                IntervalsDifference(item1.Intervals, item2.Intervals)));
                            break;
                        }
                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "difference_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, ckl1.Source, relation);
            }

            public static CKL Inversion(CKL ckl) // Инверсия отношения
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");
                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item in ckl.Relation)
                {
                    if (!IsIntervalsEmpty(item.Intervals))
                        relation.Add(new RelationItem(item.Value,
                            IntervalsInversion(item.Intervals, ckl.GlobalInterval)));

                    else relation.Add(new RelationItem(item.Value, new List<TimeInterval>()
                    { ckl.GlobalInterval}));
                }

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "inversion_" + name);

                return new CKL(newPath, ckl.GlobalInterval, ckl.Dimention, ckl.Source, relation);
            }

            public static CKL Tranposition(CKL ckl)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();
                Pair pair = new Pair();
                RelationItem currentItem;

                foreach (RelationItem item in ckl.Relation)
                {
                    if (item.Value.Values == null || item.Value.Values.Count == 0) throw new ArgumentException();

                    pair = new Pair(item.Value.Values.Reverse<object>());

                    source.Add(pair);

                    currentItem = new RelationItem(pair, item.Intervals, item.Info);
                    relation.Add(currentItem);
                }

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "transposition_" + name);

                return new CKL(newPath, ckl.GlobalInterval, ckl.Dimention, source, relation);
            }


            private static List<TimeInterval> TruncateIntervals(List<TimeInterval> intervals, Func<TimeInterval, bool> selector)
            {
                List<TimeInterval> result = new List<TimeInterval>();
                foreach (TimeInterval interval in intervals) if (selector(interval)) result.Add(interval);

                if (result.Count == 0) result.Add(TimeInterval.ZERO);

                return result;
            }

            public static CKL TruncationHighLimit(CKL ckl, double duration)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item in ckl.Relation)
                {
                    relation.Add(new RelationItem(item.Value, TruncateIntervals(item.Intervals,
                        interval => interval.Duration <= duration), item.Info));
                }

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "truncate_high_" + name);

                return new CKL(newPath, ckl.GlobalInterval, ckl.Dimention, ckl.Source, relation);
            }

            public static CKL TruncationLowLimit(CKL ckl, double duration)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object con not be null");

                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item in ckl.Relation)
                {
                    relation.Add(new RelationItem(item.Value, TruncateIntervals(item.Intervals,
                        interval => interval.Duration >= duration), item.Info));
                }

                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, "truncate_low_" + name);

                return new CKL(newPath, ckl.GlobalInterval, ckl.Dimention, ckl.Source, relation);
            }


            //Algebra Operation

            private static void TryThrowAlgebraException(CKL ckl1, CKL ckl2)
            {
                if (ckl1 == null) throw new ArgumentNullException("CKL object con not be null");
                if (ckl2 == null) throw new ArgumentNullException("CKL object con not be null");

                if (!ckl1.GlobalInterval.Equals(ckl2.GlobalInterval)) throw new ArgumentException();
                if (!ckl1.Dimention.Equals(ckl2.Dimention)) throw new ArgumentException();

                foreach (Pair p in ckl1.Source) 
                {
                    if (p.Values.Count < 2) throw new ArgumentException();
                }

                foreach (Pair p in ckl2.Source) 
                {
                    if (p.Values.Count < 2) throw new ArgumentException();
                }
            }

            private static List<TimeInterval> LeftPrecedenceCombine(List<TimeInterval> intervals1, List<TimeInterval> intervals2, double t) 
            {
				if (intervals1[0].Equals(TimeInterval.ZERO)) return [TimeInterval.ZERO];

				List<TimeInterval> res = [];

                for (int i = 0; i < intervals2.Count; i++)
                {
                    if (intervals1[0].StartTime + t < intervals2[i].EndTime) 
                        res.Add(new TimeInterval(intervals2[i].StartTime > intervals1[0].StartTime + t ? intervals2[i].StartTime : intervals1[0].StartTime + t, intervals2[i].EndTime));
                }

                if (res.Count == 0) res.Add(TimeInterval.ZERO);

                return res;
            }

            public static CKL Composition(CKL ckl1, CKL ckl2, double t)
            {
                TryThrowAlgebraException(ckl1, ckl2);

                Dictionary<string, List<RelationItem>> firstVals = new Dictionary<string, List<RelationItem>>(); 
                Dictionary<string, List<RelationItem>> lastVals = new Dictionary<string, List<RelationItem>>();

                string key;
                foreach (RelationItem item in ckl1.Relation) 
                {
                    key = item.Value.Values.Last().ToString();

                    if (!lastVals.ContainsKey(key)) lastVals.Add(key, []);
                    lastVals[key].Add(item);
                }

				foreach (RelationItem item in ckl2.Relation)
				{
					key = item.Value.Values.First().ToString();

					if (!firstVals.ContainsKey(key)) firstVals.Add(key, []);
					firstVals[key].Add(item);
				}

				HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();
                Pair pair;

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    key = item1.Value.Values.Last().ToString();

                    if (firstVals.ContainsKey(key)) 
                    {
                        foreach (RelationItem item2 in firstVals[key]) 
                        {
                            pair = new Pair(item1.Value.Values.GetRange(0, item1.Value.Values.Count - 1).Concat(item2.Value.Values));
                            source.Add(pair);
                            relation.Add(new RelationItem(pair, LeftPrecedenceCombine(TruncateIntervals(item1.Intervals, 
                                (interval) => interval.Duration >= t), item2.Intervals, t)));
                        }
                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "composition_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, source, relation);
            }

            //Semantic operations
            private static void TryThrowSemanticException(CKL ckl1, CKL ckl2)
            {
                if (ckl1 == null) throw new ArgumentNullException("CKL object con not be null");
                if (ckl2 == null) throw new ArgumentNullException("CKL object con not be null");

                if (!ckl1.GlobalInterval.Equals(ckl2.GlobalInterval)) throw new ArgumentException();
                if (!ckl1.Dimention.Equals(ckl2.Dimention)) throw new ArgumentException();

                foreach (Pair p in ckl1.Source)
                {
                    if (p.Values.Count > 1) throw new ArgumentException();
                }
                foreach (Pair p in ckl2.Source)
                {
                    if (p.Values.Count > 1) throw new ArgumentException();
                }
            }

            public static CKL SemanticUnion(CKL ckl1, CKL ckl2)
            {
                TryThrowSemanticException(ckl1, ckl2);

                HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();
                Pair p;

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    foreach (RelationItem item2 in ckl2.Relation)
                    {
                        p = new Pair([item1.Value.Values[0], item2.Value.Values[0]]);
                        source.Add(p);
                        relation.Add(new RelationItem(p, IntervalsUnion(item1.Intervals, item2.Intervals)));
                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "semantic_union_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, source, relation);
            }

            public static CKL SemanticIntersection(CKL ckl1, CKL ckl2)
            {
                TryThrowSemanticException(ckl1, ckl2);

                HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();
                Pair p;

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    foreach (RelationItem item2 in ckl2.Relation)
                    {
                        p = new Pair([item1.Value.Values[0], item2.Value.Values[0]]);
                        source.Add(p);
                        relation.Add(new RelationItem(p, IntervalsIntersection(item1.Intervals, item2.Intervals)));
                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "semantic_intersection_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, source, relation);
            }

            public static CKL SemanticDifference(CKL ckl1, CKL ckl2)
            {
                TryThrowSemanticException(ckl1, ckl2);

                HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();
                Pair p;

                foreach (RelationItem item1 in ckl1.Relation)
                {
                    foreach (RelationItem item2 in ckl2.Relation)
                    {
                        p = new Pair([item1.Value.Values[0], item2.Value.Values[0]]);
                        source.Add(p);
                        relation.Add(new RelationItem(p, IntervalsDifference(item1.Intervals, item2.Intervals)));
                    }
                }

                string file1 = Path.GetFileName(ckl1.FilePath);
                string file2 = Path.GetFileName(ckl2.FilePath);

                string name1 = file1.Substring(0, file1.LastIndexOf('.'));
                string name2 = file2.Substring(0, file2.LastIndexOf('.'));

                string newName = "semantic_difference_" + name1 + "_" + name2;
                string newFilePath = GetNewFilePath(ckl1.FilePath, newName);

                return new CKL(newFilePath, ckl1.GlobalInterval, ckl1.Dimention, source, relation);
            }

            // Dop operations
            public static CKL ItemProjection(CKL ckl, object current, Func<object, object, bool> comp, TimeInterval interval)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object could not be null");

                if (interval == null) throw new ArgumentNullException("Time Interval shouldn't be null");

                if (current == null) throw new ArgumentNullException("Item of relation cannot be null");

                HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item in ckl.Relation)
                {
                    if (item.Value.HasObject(current, comp) && !item.Intervals[0].Equals(TimeInterval.ZERO) &&
                        IsIntervalInserted(interval, item.Intervals))
                    {
                        source.Add(item.Value);
                        relation.Add(new RelationItem(item.Value, new List<TimeInterval>() { (TimeInterval)interval.Clone() }));
                    }
                }


                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, $"proj_{current.ToString()}_" +
                    $"{(interval.StartTime.ToString().IndexOf('.') == -1 ?
                    interval.StartTime.ToString() : interval.StartTime.ToString().Substring(0, interval.EndTime.ToString().IndexOf('.')))}" +
                    $"{(interval.EndTime.ToString().IndexOf('.') == -1 ?
                    interval.EndTime.ToString() : interval.EndTime.ToString().Substring(0, interval.EndTime.ToString().IndexOf('.')))}" +
                    name);

                return new CKL(newPath, interval, ckl.Dimention, source, relation);
            }

            private static bool ContainsMany(Pair p, List<object> currents, Func<object, object, bool> comp)
            {
                foreach (object obj in currents) if (!p.HasObject(obj, comp)) return false;

                return true;
            }

            public static CKL ItemProjection(CKL ckl, List<object> currents, Func<object, object, bool> comp, TimeInterval interval)
            {
                if (ckl == null) throw new ArgumentNullException("CKL object could not be null");

                if (interval == null) throw new ArgumentNullException("Time Interval shouldn't be null");

                if (currents == null || currents.Contains(null)) throw new ArgumentNullException("Items can not be null");


                HashSet<Pair> source = new HashSet<Pair>();
                HashSet<RelationItem> relation = new HashSet<RelationItem>();

                foreach (RelationItem item in ckl.Relation)
                {
                    if (ContainsMany(item.Value, currents, comp) && !item.Intervals[0].Equals(TimeInterval.ZERO) &&
                        IsIntervalInserted(interval, item.Intervals))
                    {
                        source.Add(item.Value);
                        relation.Add(new RelationItem(item.Value, new List<TimeInterval>() { (TimeInterval)interval.Clone() }));
                    }
                }


                string file = Path.GetFileName(ckl.FilePath);
                string name = file.Substring(0, file.LastIndexOf('.'));

                string newPath = GetNewFilePath(ckl.FilePath, $"proj_{string.Join('_', currents)}_" +
                    $"{(interval.StartTime.ToString().IndexOf('.') == -1 ?
                    interval.StartTime.ToString() : interval.StartTime.ToString().Substring(0, interval.EndTime.ToString().IndexOf('.')))}_" +
                    $"{(interval.EndTime.ToString().IndexOf('.') == -1 ?
                    interval.EndTime.ToString() : interval.EndTime.ToString().Substring(0, interval.EndTime.ToString().IndexOf('.')))}_" +
                    name);

                return new CKL(newPath, interval, ckl.Dimention, source, relation);
            }

            private static bool AreCKLGroupable(CKL ckl1, CKL ckl2)
            {
                if (ckl1 == null && ckl2 == null) return true;

                if (ckl1 == null || ckl2 == null) return false;

                if (ckl1.Dimention != ckl2.Dimention) return false;

                CKL cond = new CKL(ckl2.FilePath, ckl1.GlobalInterval, ckl2.Dimention, ckl2.Source, ckl2.Relation);

                try
                {
                    TryThrowSemanticException(ckl1, cond);
                    return true;
                }
                catch { }

                try
                {
                    TryThrowBinaryExceptions(ckl1, cond);
                    return true;
                }
                catch { }

                try
                {
                    TryThrowAlgebraException(ckl1, cond);
                    return true;
                }
                catch { }

                return false;
            }

            public static List<List<CKL>> GroupByTheme(List<CKL> diagrams, Func<CKL, CKL, bool> filter)
            {
                List<List<CKL>> result = new List<List<CKL>>();

                if (diagrams == null || diagrams.Count == 0) return result;

                result.Add([diagrams[0]]);
                bool isAdded = false;

                foreach (CKL ckl in diagrams.GetRange(1, diagrams.Count - 1))
                {
                    foreach (List<CKL> potential in result)
                    {
                        foreach (CKL cond in potential)
                        {
                            if (filter(ckl, cond))
                            {
                                potential.Add(ckl);
                                isAdded = true;
                                break;
                            }
                        }
                    }

                    if (!isAdded) result.Add([ckl]);
                    isAdded = false;
                }

                return result;
            }

            public static List<List<CKL>> GroupByTheme(List<CKL> diagrams)
            {
                return GroupByTheme(diagrams, AreCKLGroupable);
            }
        }
    }
}
