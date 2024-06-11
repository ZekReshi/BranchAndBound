using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    internal record BnBBenchmarkResult(int NTasks, double Time)
    {
        public static List<BnBBenchmarkResult> GroupAndAverage(IEnumerable<BnBBenchmarkResult> results)
        {
            return [..results
                .GroupBy(res => res.NTasks)
                .Select(group => new BnBBenchmarkResult(group.Key, group.Average(res => res.Time)))
                .OrderBy(res => res.NTasks)];
        }
    }
}
