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
        private Queue<IBnBProblem> Q { get; init; }
        public IBnBProblem? GlobalBest { get; private set; }
        private object GlobalBestLock { get; init; }

        public BnB(IBnBProblem rootProblem, int nThreads = 8)
        {
            Q = new Queue<IBnBProblem>();
            Q.Enqueue(rootProblem);
            nTasks = nThreads;
            GlobalBest = null;
            GlobalBestLock = new object();
        }

        public async Task Run()
        {
            Task[] tasks = new Task[nTasks];
            CancellationTokenSource cancellationTokenSource = new();
            Progress<IBnBProblem> progress = new(problem => Console.Write(problem.ToString() + '\r'));
            for (int i = 0; i < nTasks; i++)
            {
                tasks[i] = new Task(() => Execute(cancellationTokenSource.Token, progress));
                tasks[i].Start();
            }
            while (!Console.KeyAvailable)
            {
                int completed = 0;
                foreach (Task task in tasks)
                    if (task.IsCompleted)
                        completed++;
                if (completed == tasks.Length) break;
                Thread.Sleep(100);
            }
            cancellationTokenSource.Cancel();
        }

        public void Execute(CancellationToken cancellationToken, IProgress<IBnBProblem> progress)
        {
            Stack<IBnBProblem> stack = new();

            IBnBProblem? personalBest = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (stack.Count == 0)
                {
                    lock(Q)
                    {
                        if (Q.Count == 1 && !Q.Peek().IsLeaf())
                        {
                            IBnBProblem lastProblem = Q.Dequeue();
                            foreach (IBnBProblem newProblem in lastProblem.Branch(personalBest))
                            {
                                Q.Enqueue(newProblem);
                            }
                        }
                        if (Q.Count == 0)
                        {
                            break;
                        }
                        stack.Push(Q.Dequeue());
                    }
                }
                IBnBProblem problem = stack.Pop();
                if (!problem.IsLeaf())
                {
                    foreach (IBnBProblem newProblem in problem.Branch(personalBest))
                    {
                        stack.Push(newProblem);
                    }
                }
                else if (personalBest == null || problem > personalBest)
                {
                    lock(GlobalBestLock)
                    {
                        personalBest = GlobalBest;
                        if (GlobalBest == null || problem > GlobalBest)
                        {
                            GlobalBest = problem;
                            progress.Report(problem);
                        }
                    }
                }
            }
        }
    }
}
