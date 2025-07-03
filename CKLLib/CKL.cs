using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CKLLib.Operations;

namespace CKLLib
{
    public class CKL // объект алгебры динамических отношений
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string FilePath { get; set; } // путь файла, в котором записан объект
        public TimeInterval GlobalInterval { get; set; } // интервал времени,
                                                         // на котором определено отношение
        public TimeDimentions Dimention { get; set; } // еденица измерения времени интервала
        public HashSet<Pair> Source { get; set; } // множество, на котором задано отношение
        public HashSet<RelationItem> Relation
        {
            get => _relation;
            set
            {
                _relation = value;
                FillRelation();
            }
        } // динамическое отношение

        private HashSet<RelationItem> _relation;

        public CKL()
        {
            FilePath = string.Empty;
            GlobalInterval = TimeInterval.ZERO;
            Dimention = TimeDimentions.SECONDS;
            Source = new HashSet<Pair>();
            _relation = new HashSet<RelationItem>();
        }

        public CKL(string filePath, TimeInterval timeInterval, TimeDimentions dimention, HashSet<Pair> source, HashSet<RelationItem> relation)
        {
            FilePath = filePath;
            GlobalInterval = timeInterval;
            Dimention = dimention;
            Source = source;
            _relation = relation;

            FillRelation();
        }

        private void FillRelation()
        {
            List<RelationItem> extra = [];
            foreach (RelationItem item in _relation)
            {
                if (!Source.Contains(item.Value, new Pair.PairEqualityComparer())) extra.Add(item);
            }

            foreach (RelationItem item in extra) _relation.Remove(item);

            extra.Clear();

            foreach (Pair item in Source)
            {
                if (!_relation.Any(x => x.Value.Equals(item))) _relation.Add(new RelationItem(item, [TimeInterval.ZERO]));
            }

            HashSet<Pair> data = [];

            foreach (RelationItem item in _relation)
            {
                if (data.Contains(item.Value, new Pair.PairEqualityComparer())) extra.Add(item);
                else data.Add(item.Value);
            }

            foreach (RelationItem item in extra) _relation.Remove(item);
        }

        public override bool Equals(object? obj)
        {
            CKL? ckl = obj as CKL;
            if (ckl is null) return false;


            return GlobalInterval.Equals(ckl.GlobalInterval) && Dimention.Equals(ckl.Dimention)
                && Source.SetEquals(ckl.Source) && _relation.SetEquals(ckl.Relation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GlobalInterval, Dimention, Source, _relation);
        }

        public static void Save(CKL ckl)
        {
            string s = JsonSerializer.Serialize(ckl);
            File.WriteAllText(ckl.FilePath, s);
        }

        public static CKL? GetFromFile(string path)
        {
            CKL? ckl = JsonSerializer.Deserialize<CKL>(File.ReadAllText(path));
            if (ckl is not null) ckl.FilePath = path;
            return ckl;
        }
    }
}
