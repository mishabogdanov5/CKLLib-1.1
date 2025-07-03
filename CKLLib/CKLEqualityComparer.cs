using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKLLib
{
    public class CKLEqualityComparer : IEqualityComparer<CKL>
    {
        public bool Equals(CKL? x, CKL? y)
        {
            if (x == null && y == null) return true;
            if ((x == null && y != null) || (x != null && y == null)) return false;

            if (!x.GlobalInterval.Equals(y.GlobalInterval)) return false;

            if (!x.Dimention.Equals(y.Dimention)) return false;

            if (!x.Source.SequenceEqual(y.Source)) return false;

            if (!x.Relation.SequenceEqual(y.Relation)) return false;

            return true;
        }


        public bool AbsoluteEquals(CKL? x, CKL? y)
        {
            if (x == null && y == null) return true;

            return Equals(x, y) && x.FilePath.Equals(y.FilePath);
        }

        public int GetHashCode([DisallowNull] CKL obj)
        {
            return obj.GetHashCode();
        }
    }
}
