using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    public interface IBnBProblem : IComparable<IBnBProblem>
    {
        public IEnumerable<IBnBProblem> Branch(IBnBProblem? best);
        public bool IsLeaf();
        void PrintProblem();

        public static bool operator <(IBnBProblem left, IBnBProblem right) => left.CompareTo(right) < 0;
        public static bool operator >(IBnBProblem left, IBnBProblem right) => left.CompareTo(right) > 0;
        public static bool operator <=(IBnBProblem left, IBnBProblem right) => left.CompareTo(right) <= 0;
        public static bool operator >=(IBnBProblem left, IBnBProblem right) => left.CompareTo(right) >= 0;
    }
}
