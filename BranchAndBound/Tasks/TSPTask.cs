using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound.Tasks
{
    public class TSPTask(int[,] distances, int[] nodes) : IBnBTask
    {
        public List<IBnBTask> Branch(IBnBTask? best)
        {
            List<IBnBTask> children = [];
            if (nodes.Length == distances.GetLength(0))
            {
                if (distances[nodes.Last(), 0] != 0)
                {
                    int[] newNodes = new int[nodes.Length + 1];
                    Array.Copy(nodes, newNodes, nodes.Length);
                    newNodes[nodes.Length] = 0;
                    children.Add(new TSPTask(distances, newNodes));
                }
                return children;
            }
            for (int i = 1; i < distances.GetLength(0); i++)
            {
                if (distances[nodes.Last(), i] == 0 || nodes.Contains(i)) continue;
                int[] newNodes = new int[nodes.Length + 1];
                Array.Copy(nodes, newNodes, nodes.Length);
                newNodes[nodes.Length] = i;
                TSPTask newTask = new(distances, newNodes);
                if (best == null || newTask.CompareTo(best) > 0)
                {
                    children.Add(newTask);
                }
            }
            return children;
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

        public override string ToString()
        {
            return $"Distance: {Distance()}, Nodes: {string.Join(' ', nodes)}";
        }
    }
}
