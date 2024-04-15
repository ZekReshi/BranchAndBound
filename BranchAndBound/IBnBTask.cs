using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    public interface IBnBTask : IComparable<IBnBTask>
    {
        public List<IBnBTask> Branch(IBnBTask? best);
        public bool IsLeaf();
    }
}
