using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BranchAndBound.Tasks
{
    public class QAPTask(int[,] flows, int[,] distances, int[] assignedLocations) : IBnBTask
    {
        public List<IBnBTask> Branch(IBnBTask? best)
        {
            List<IBnBTask> children = [];
            if (assignedLocations.Length < flows.GetLength(0))
            {
                for (int i = 0; i < flows.GetLength(0); i++)
                {
                    if (assignedLocations.Contains(i)) continue;
                    int[] newAssignedLocations = new int[assignedLocations.Length + 1];
                    Array.Copy(assignedLocations, newAssignedLocations, assignedLocations.Length);
                    newAssignedLocations[assignedLocations.Length] = i;
                    QAPTask newTask = new(flows, distances, newAssignedLocations);
                    if (best == null || newTask.CompareTo(best) > 0)
                    {
                        children.Add(newTask);
                    }
                }
            }
            return children;
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

        public override string ToString()
        {
            return $"FlowDistance: {FlowDistance()}, Assigned Locations: {string.Join(' ', assignedLocations)}";
        }
    }
}
