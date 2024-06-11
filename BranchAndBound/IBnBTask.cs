using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    public interface IBnBTask : IComparable<IBnBTask>
    {
        public IEnumerable<IBnBTask> Branch(IBnBTask? best);
        public bool IsLeaf();
        void PrintProblem();

        public static bool operator <(IBnBTask left, IBnBTask right) => left.CompareTo(right) < 0;
        public static bool operator >(IBnBTask left, IBnBTask right) => left.CompareTo(right) > 0;
        public static bool operator <=(IBnBTask left, IBnBTask right) => left.CompareTo(right) <= 0;
        public static bool operator >=(IBnBTask left, IBnBTask right) => left.CompareTo(right) >= 0;
    }
}
