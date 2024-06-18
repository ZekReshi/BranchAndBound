using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace BranchAndBound.Tasks
{
    public class FSSTask : IBnBTask
    {
        readonly int[,] tasks;
        readonly int[] schedule;

        public FSSTask(int size)
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

        public FSSTask(int[,] tasks)
        {
            this.tasks = tasks;
            this.schedule = [];
        }

        private FSSTask(int[,] tasks, int[] schedule)
        {
            this.tasks = tasks;
            this.schedule = schedule;
        }

        public IEnumerable<IBnBTask> Branch(IBnBTask? best)
        {
            if (schedule.Length < tasks.GetLength(0))
            {
                for (int i = 0; i < tasks.GetLength(0); i++)
                {
                    if (schedule.Contains(i)) continue;
                    int[] newSchedule = new int[schedule.Length + 1];
                    Array.Copy(schedule, newSchedule, schedule.Length);
                    newSchedule[schedule.Length] = i;
                    FSSTask newTask = new(tasks, newSchedule);
                    if (best == null || newTask > best)
                    {
                        yield return newTask;
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

        public int CompareTo(IBnBTask? other)
        {
            if (other is FSSTask task)
            {
                return -Time().CompareTo(task.Time());
            }
            throw new ArgumentException("Cannot compare two different IBnBTasks");
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
