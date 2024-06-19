using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound.Problems
{
    public class TSPProblem : IBnBProblem
    {
        readonly int[,] distances;
        readonly int[] nodes;

        public TSPProblem(int size)
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

        public TSPProblem(int[,] distances)
        {
            this.distances = distances;
            this.nodes = [0];
        }

        private TSPProblem(int[,] distances, int[] nodes)
        {
            this.distances = distances;
            this.nodes = nodes;
        }

        public IEnumerable<IBnBProblem> Branch(IBnBProblem? best)
        {
            if (nodes.Length == distances.GetLength(0))
            {
                if (distances[nodes.Last(), 0] != 0)
                {
                    int[] newNodes = new int[nodes.Length + 1];
                    Array.Copy(nodes, newNodes, nodes.Length);
                    newNodes[nodes.Length] = 0;
                    yield return new TSPProblem(distances, newNodes);
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
                    TSPProblem newProblem = new(distances, newNodes);
                    if (best == null || newProblem > best)
                    {
                        yield return newProblem;
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

        public int CompareTo(IBnBProblem? other)
        {
            if (other is TSPProblem problem)
            {
                return -Distance().CompareTo(problem.Distance());
            }
            throw new ArgumentException("Cannot compare two different IBnBProblems");
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
