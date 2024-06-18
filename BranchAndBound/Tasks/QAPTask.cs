using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BranchAndBound.Tasks
{
    public class QAPTask : IBnBTask
    {
        readonly int[,] flows;
        readonly int[,] distances;
        readonly int[] assignedLocations;

        public QAPTask(int size)
        {
            Random random = new();
            flows = new int[size, size];
            for (int i = 0; i < flows.GetLength(0); i++)
            {
                for (int j = 0; j < flows.GetLength(1); j++)
                {
                    flows[i, j] = random.Next(8) + 1;
                }
            }
            distances = new int[size, size];
            for (int i = 0; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances.GetLength(1); j++)
                {
                    distances[i, j] = random.Next(8) + 1;
                }
            }
            assignedLocations = [];
        }

        public QAPTask(int[,] flows, int[,] distances)
        {
            this.flows = flows;
            this.distances = distances;
            this.assignedLocations = [];
        }

        private QAPTask(int[,] flows, int[,] distances, int[] assignedLocations)
        {
            this.flows = flows;
            this.distances = distances;
            this.assignedLocations = assignedLocations;
        }

        public IEnumerable<IBnBTask> Branch(IBnBTask? best)
        {
            if (assignedLocations.Length < flows.GetLength(0))
            {
                for (int i = 0; i < flows.GetLength(0); i++)
                {
                    if (assignedLocations.Contains(i)) continue;
                    int[] newAssignedLocations = new int[assignedLocations.Length + 1];
                    Array.Copy(assignedLocations, newAssignedLocations, assignedLocations.Length);
                    newAssignedLocations[assignedLocations.Length] = i;
                    QAPTask newTask = new(flows, distances, newAssignedLocations);
                    if (best == null || newTask > best)
                    {
                        yield return newTask;
                    }
                }
            }
        }

        public int FlowDistance()
        {
            int flowDistances = 0;
            for (int i = 0; i < assignedLocations.Length; i++)
            {
                for (int j = 0; j < assignedLocations.Length; j++)
                {
                    flowDistances += flows[i, j] * distances[assignedLocations[i], assignedLocations[j]];
                }
            }
            return flowDistances;
        }

        public int CompareTo(IBnBTask? other)
        {
            if (other is QAPTask task)
            {
                return -FlowDistance().CompareTo(task.FlowDistance());
            }
            throw new ArgumentException("Cannot compare two different IBnBTasks");
        }

        public bool IsLeaf()
        {
            return assignedLocations.Length == flows.GetLength(0);
        }

        public void PrintProblem()
        {
            Console.WriteLine("Flows:");
            for (int i = 0; i < flows.GetLength(0); i++)
            {
                for (int j = 0; j < flows.GetLength(1); j++)
                {
                    Console.Write(flows[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Distances:");
            for (int i = 0; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances.GetLength(1); j++)
                {
                    Console.Write(distances[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            return $"FlowDistance: {FlowDistance()}, Assigned Locations: {string.Join(' ', assignedLocations)}";
        }
    }
}
