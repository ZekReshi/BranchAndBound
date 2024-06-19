using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BranchAndBound.Problems
{
    public class QAPProblem : IBnBProblem
    {
        readonly int[,] flows;
        readonly int[,] distances;
        readonly int[] assignedLocations;

        public QAPProblem(int size)
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

        public QAPProblem(int[,] flows, int[,] distances)
        {
            this.flows = flows;
            this.distances = distances;
            this.assignedLocations = [];
        }

        private QAPProblem(int[,] flows, int[,] distances, int[] assignedLocations)
        {
            this.flows = flows;
            this.distances = distances;
            this.assignedLocations = assignedLocations;
        }

        public IEnumerable<IBnBProblem> Branch(IBnBProblem? best)
        {
            if (assignedLocations.Length < flows.GetLength(0))
            {
                for (int i = 0; i < flows.GetLength(0); i++)
                {
                    if (assignedLocations.Contains(i)) continue;
                    int[] newAssignedLocations = new int[assignedLocations.Length + 1];
                    Array.Copy(assignedLocations, newAssignedLocations, assignedLocations.Length);
                    newAssignedLocations[assignedLocations.Length] = i;
                    QAPProblem newProblem = new(flows, distances, newAssignedLocations);
                    if (best == null || newProblem > best)
                    {
                        yield return newProblem;
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

        public int CompareTo(IBnBProblem? other)
        {
            if (other is QAPProblem problem)
            {
                return -FlowDistance().CompareTo(problem.FlowDistance());
            }
            throw new ArgumentException("Cannot compare two different IBnBProblems");
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
