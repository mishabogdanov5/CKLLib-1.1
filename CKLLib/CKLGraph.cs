using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKLLib
{
    public class CKLGraph
    {
        public CKL CurrentRelation { get => _ckl; }
        private CKL _ckl;

        private Dictionary<TimeInterval, HashSet<Pair>> _data;
        private Dictionary<int, TimeInterval> _diapasones;

        public CKLGraph(CKL ckl)
        {
            if (ckl == null) throw new ArgumentNullException("CKL can not be null");
            _ckl = ckl;

            InitGraph();
        }

        private void InitGraph()
        {
            _data = new Dictionary<TimeInterval, HashSet<Pair>>();
            _diapasones = new Dictionary<int, TimeInterval>();

            Dictionary<double, HashSet<Pair>> startTimeTriggers = new Dictionary<double, HashSet<Pair>>() { { _ckl.GlobalInterval.StartTime, [] } };
            Dictionary<double, HashSet<Pair>> endTimeTriggers = new Dictionary<double, HashSet<Pair>>() { { _ckl.GlobalInterval.EndTime, [] } };

            foreach (RelationItem item in _ckl.Relation)
            {
                foreach (TimeInterval interval in item.Intervals)
                {
                    if (!startTimeTriggers.ContainsKey(interval.StartTime)) startTimeTriggers.Add(interval.StartTime, []);
                    if (!endTimeTriggers.ContainsKey(interval.EndTime)) endTimeTriggers.Add(interval.EndTime, []);

                    startTimeTriggers[interval.StartTime].Add(item.Value);
                    endTimeTriggers[interval.EndTime].Add(item.Value);
                }
            }

            double[] starts = startTimeTriggers.Keys.Order().ToArray();
            double[] ends = endTimeTriggers.Keys.Order().ToArray();

            HashSet<Pair> next = startTimeTriggers[starts[0]];
            HashSet<Pair> curr = [];

            int i = 1;
            int j = 0;

            double start = 0;
            double end = starts[0];

            int dStart = 0;
            int dEnd = 0;

            while (i < starts.Length || j < ends.Length)
            {
                curr = [.. next];
                start = end;

                if (j >= ends.Length || (i < starts.Length && ends[j] > starts[i]))
                {
                    end = starts[i];
                    foreach (Pair p in startTimeTriggers[end]) next.Add(p);

                    i++;
                }

                else
                {
                    end = ends[j];
                    foreach (Pair p in endTimeTriggers[end]) next.Remove(p);

                    j++;
                }

                TimeInterval interval = new TimeInterval(start, end);
                _data.Add(interval, curr);

                dStart = (int)start;
                dEnd = (int)end;

                if ((int)start != start) dStart++;
                if ((int)end != end || end == _ckl.GlobalInterval.EndTime) dEnd++;

                for (int d = dStart; d < dEnd; d++) _diapasones.Add(d, interval);
            }
        }

        public HashSet<Pair> GetGraphByTime(int time)
        {
            if (time < _ckl.GlobalInterval.StartTime || time > _ckl.GlobalInterval.EndTime) throw new ArgumentOutOfRangeException("time is out of global ckl range");

            return _data[_diapasones[time]];
        }

        public HashSet<Pair> GetGraphByTime(double time)
        {
            foreach (TimeInterval interval in _data.Keys)
            {
                if (interval.StartTime <= time && interval.EndTime > time)
                {
                    return _data[interval];
                }
            }

            return [];
        }
    }
}
