using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace BranchAndBound.Problems
{
    public class FSSProblem : IBnBProblem
    {
        readonly int[,] tasks;
        readonly int[] schedule;

        public FSSProblem(int size)
        {
            Random random = new();
            tasks = new int[size, size];
            for (int i = 0; i < tasks.GetLength(0); i++)
            {
                for (int j = 0; j < tasks.GetLength(1); j++)
                {
                    tasks[i, j] = random.Next(8) + 1;
                }
            }
            schedule = [];
        }

        public FSSProblem(int[,] tasks)
        {
            this.tasks = tasks;
            this.schedule = [];
        }

        private FSSProblem(int[,] tasks, int[] schedule)
        {
            this.tasks = tasks;
            this.schedule = schedule;
        }

        public IEnumerable<IBnBProblem> Branch(IBnBProblem? best)
        {
            if (schedule.Length < tasks.GetLength(0))
            {
                for (int i = 0; i < tasks.GetLength(0); i++)
                {
                    if (schedule.Contains(i)) continue;
                    int[] newSchedule = new int[schedule.Length + 1];
                    Array.Copy(schedule, newSchedule, schedule.Length);
                    newSchedule[schedule.Length] = i;
                    FSSProblem newProblem = new(tasks, newSchedule);
                    if (best == null || newProblem > best)
                    {
                        yield return newProblem;
                    }
                }
            }
        }

        public int Time()
        {
            if (schedule.Length == 0) return 0;
            int[] times = new int[schedule.Length];
            for (int i = 0; i < tasks.GetLength(1); i++)
            {
                times[0] += tasks[schedule[0], i];
                for (int j = 1; j < schedule.Length; j++)
                {
                    times[j] = Math.Max(times[j], times[j-1]) + tasks[schedule[j], i];
                }
            }
            return times[schedule.Length - 1];
        }

        public int CompareTo(IBnBProblem? other)
        {
            if (other is FSSProblem problem)
            {
                return -Time().CompareTo(problem.Time());
            }
            throw new ArgumentException("Cannot compare two different IBnBProblems");
        }

        public bool IsLeaf()
        {
            return schedule.Length == tasks.GetLength(0);
        }

        public void PrintProblem()
        {
            Console.WriteLine("Tasks:");
            for (int i = 0; i < tasks.GetLength(0); i++)
            {
                for (int j = 0; j < tasks.GetLength(1); j++)
                {
                    Console.Write(tasks[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            return $"Time: {Time()}, Schedule: {string.Join(' ', schedule)}";
        }
    }
}
