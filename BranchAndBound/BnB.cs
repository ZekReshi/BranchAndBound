using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    public class BnB
    {
        private readonly int nTasks;
        private Queue<IBnBTask> Q { get; init; }
        public IBnBTask? GlobalBest { get; private set; }
        private object GlobalBestLock { get; init; }

        public BnB(IBnBTask rootTask, int nThreads = 8)
        {
            Q = new Queue<IBnBTask>();
            Q.Enqueue(rootTask);
            nTasks = nThreads;
            GlobalBest = null;
            GlobalBestLock = new object();
        }

        public async Task Run()
        {
            Task[] tasks = new Task[nTasks];
            for (int i = 0; i < nTasks; i++)
            {
                tasks[i] = new Task(Execute);
                tasks[i].Start();
            }
            for (int i = 0; i < nTasks; i++)
            {
                await tasks[i];
            }
        }

        public void Execute()
        {
            Stack<IBnBTask> stack = new();

            IBnBTask? personalBest = null;

            do
            {
                if (stack.Count == 0)
                {
                    lock(Q)
                    {
                        if (Q.Count == 1 && !Q.Peek().IsLeaf())
                        {
                            IBnBTask lastTask = Q.Dequeue();
                            foreach (IBnBTask newTask in lastTask.Branch(personalBest))
                            {
                                Q.Enqueue(newTask);
                            }
                        }
                        if (Q.Count == 0)
                        {
                            break;
                        }
                        stack.Push(Q.Dequeue());
                    }
                }
                IBnBTask task = stack.Pop();
                if (!task.IsLeaf())
                {
                    foreach (IBnBTask newTask in task.Branch(personalBest))
                    {
                        stack.Push(newTask);
                    }
                }
                else if (personalBest == null || task > personalBest)
                {
                    lock(GlobalBestLock)
                    {
                        personalBest = GlobalBest;
                        if (GlobalBest == null || task > GlobalBest)
                        {
                            GlobalBest = task;
                        }
                    }
                }
            } while (true);
        }
    }
}
