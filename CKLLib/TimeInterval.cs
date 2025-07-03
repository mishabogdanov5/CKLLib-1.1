using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CKLLib.Operations;

namespace CKLLib
{
    public class TimeInterval : IComparable<TimeInterval>, ICloneable // временной интервал
    {
        public double StartTime
        {
            get => _startTime;  // время начала
            set
            {
                if (value <= _endTime) _startTime = value;
                else _startTime = _endTime;
            }
        }
        public double EndTime
        {
            get => _endTime; // время конца
            set
            {
                if (value >= _startTime) _endTime = value;
                else _endTime = _startTime;
            }
        }

        private double _startTime;
        private double _endTime;

        public void Scale(double multi)
        {
            _startTime *= multi;
            _endTime *= multi;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double Duration
        {
            get
            {
                return _endTime - _startTime;
            }
        }

        public TimeInterval(double startTime, double endTime)
        {
            _startTime = startTime;
            _endTime = endTime;
        }

        public override bool Equals(object? obj)
        {
            TimeInterval? interval = obj as TimeInterval;
            if (interval == null) return false;

            if (interval.Duration == 0 && Duration == 0) return true;

            return _startTime == interval.StartTime && _endTime == interval.EndTime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_startTime, _endTime);
        }

        public override string ToString()
        {
            return $"[{Math.Round(_startTime, 2).ToString()}; {Math.Round(_endTime, 2).ToString()}]";
        }

        public int CompareTo(TimeInterval? other)
        {
            if (other == null) return 1;

            return Duration.CompareTo(other.Duration);
        }

        public object Clone()
        {
            TimeInterval newInterval = new TimeInterval(StartTime, EndTime);
            return newInterval;
        }

        public static readonly TimeInterval ZERO = new TimeInterval(0, 0);
        public static TimeInterval GetIntervalInAnotherDemention(TimeInterval interval, TimeDimentions oldDimention, TimeDimentions newDimention)
        {
            if (interval == null) return ZERO;

            int oldDim = (int)oldDimention;
            int newDim = (int)newDimention;
            double intervalMulti = 1;

            TimeInterval res = interval.Clone() as TimeInterval;

            if (oldDim > newDim)
            {
                for (int i = 0; i < oldDim - newDim; i++)
                {
                    intervalMulti *= Constants.TIME_DIMENTIONS_CONVERT[newDim + i];
                }

            }
            else
            {
                for (int i = 0; i < newDim - oldDim; i++)
                {
                    intervalMulti /= Constants.TIME_DIMENTIONS_CONVERT[oldDim + i];
                }
            }

            res.Scale(intervalMulti);
            return res;
        }
    }
}
