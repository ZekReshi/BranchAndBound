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
            CancellationTokenSource cancellationTokenSource = new();
            Progress<IBnBTask> progress = new(task => Console.Write(task.ToString() + '\r'));
            for (int i = 0; i < nTasks; i++)
            {
                tasks[i] = new Task(() => Execute(cancellationTokenSource.Token, progress));
                tasks[i].Start();
            }
            while (true)
            {
                if (Console.KeyAvailable) break;
                int completed = 0;
                foreach (Task task in tasks)
                    if (task.IsCompleted)
                        completed++;
                if (completed == tasks.Length) break;
                Thread.Sleep(100);
            }
            cancellationTokenSource.Cancel();
        }

        public void Execute(CancellationToken cancellationToken, IProgress<IBnBTask> progress)
        {
            Stack<IBnBTask> stack = new();

            IBnBTask? personalBest = null;

            while (!cancellationToken.IsCancellationRequested)
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
                            progress.Report(task);
                        }
                    }
                }
            }
        }
    }
}
