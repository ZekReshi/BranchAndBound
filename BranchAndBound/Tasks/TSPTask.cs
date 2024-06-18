using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound.Tasks
{
    public class TSPTask : IBnBTask
    {
        readonly int[,] distances;
        readonly int[] nodes;

        public TSPTask(int size)
        {
            Random random = new();
            distances = new int[size, size];
            for (int i = 0; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances.GetLength(1); j++)
                {
                    distances[i, j] = random.Next(8) + 1;
                }
            }
            nodes = [0];
        }

        public TSPTask(int[,] distances)
        {
            this.distances = distances;
            this.nodes = [0];
        }

        private TSPTask(int[,] distances, int[] nodes)
        {
            this.distances = distances;
            this.nodes = nodes;
        }

        public IEnumerable<IBnBTask> Branch(IBnBTask? best)
        {
            if (nodes.Length == distances.GetLength(0))
            {
                if (distances[nodes.Last(), 0] != 0)
                {
                    int[] newNodes = new int[nodes.Length + 1];
                    Array.Copy(nodes, newNodes, nodes.Length);
                    newNodes[nodes.Length] = 0;
                    yield return new TSPTask(distances, newNodes);
                }
            }
            else
            {
                for (int i = 1; i < distances.GetLength(0); i++)
                {
                    if (distances[nodes.Last(), i] == 0 || nodes.Contains(i)) continue;
                    int[] newNodes = new int[nodes.Length + 1];
                    Array.Copy(nodes, newNodes, nodes.Length);
                    newNodes[nodes.Length] = i;
                    TSPTask newTask = new(distances, newNodes);
                    if (best == null || newTask > best)
                    {
                        yield return newTask;
                    }
                }
            }
        }

        public int Distance()
        {
            int distance = 0;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                distance += distances[nodes[i], nodes[i + 1]];
            }
            return distance;
        }

        public int CompareTo(IBnBTask? other)
        {
            if (other is TSPTask task)
            {
                return -Distance().CompareTo(task.Distance());
            }
            throw new ArgumentException("Cannot compare two different IBnBTasks");
        }

        public bool IsLeaf()
        {
            return nodes.Length == distances.GetLength(0) + 1;
        }

        public void PrintProblem()
        {
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
            return $"Distance: {Distance()}, Nodes: {string.Join(' ', nodes)}";
        }
    }
}
