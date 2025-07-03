using System.Diagnostics.CodeAnalysis;


namespace CKLLib
{
    public class Pair
    {
        public List<object> Values { get; set; }

        public Pair()
        {
            Values = new List<object>();
        }

        public Pair(IEnumerable<object> values)
        {
            if (values == null) throw new ArgumentNullException("Values collection can not be null");
            if (values.Count() == 0) throw new ArgumentException("Values can not be empty");

            Values = values.ToList();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Pair) return false;

            Pair? pair = obj as Pair;
            if (pair == null) return false;

            return pair.Values.SequenceEqual(Values, new ValuesEqComparer());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Values);
        }

        public override string ToString()
        {
            if (Values != null)
            {
                string res = "(";

                foreach (object value in Values)
                {
                    res += $"{value};";
                }

                if (res.Length > 1)
                {
                    res = res.Substring(0, res.Length - 1);
                    res += ")";
                }

                return res;
            }

            return string.Empty;
        }

        public class PairEqualityComparer : IEqualityComparer<Pair>
        {
            public bool Equals(Pair? x, Pair? y)
            {
                if (x == null && y == null) return true;
                if (x == null) return false;

                return x.Equals(y);

            }

            public int GetHashCode([DisallowNull] Pair obj)
            {
                return obj.GetHashCode();
            }
        }

        public bool HasObject(object obj, Func<object, object, bool> comp)
        {
            if (Values != null)
            {
                foreach (object value in Values) if (comp(value, obj)) return true;
            }
            return false;
        }

        private class ValuesEqComparer : IEqualityComparer<object>
        {
            public new bool Equals(object? x, object? y)
            {
                if (x == null || y == null) return true;
                if (x == null) return false;

                return x.ToString()!.Equals(y.ToString());
            }

            public int GetHashCode([DisallowNull] object obj)
            {
                return obj.GetHashCode();
            }
        }
    }


}
