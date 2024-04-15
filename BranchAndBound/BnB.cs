using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAndBound
{
    public class BnB
    {
        private readonly int nThreads;
        private Queue<IBnBTask> Q { get; init; }
        private IBnBTask? GlobalBest {  get; set; }
        private object GlobalBestLock { get; init; }

        public BnB(IBnBTask rootTask, int nThreads)
        {
            Q = new Queue<IBnBTask>();
            Q.Enqueue(rootTask);
            this.nThreads = nThreads;
            GlobalBest = null;
            GlobalBestLock = new object();
        }

        public void Run()
        {
            List<Task> tasks = [];
            for (int thread = 0; thread < nThreads; thread++)
            {
                tasks.Add(new Task(Execute));
            }

            tasks.ForEach(t => t.Start());
            tasks.ForEach(t => t.Wait());

            Console.WriteLine(GlobalBest);
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
                            lastTask.Branch(personalBest).ForEach(Q.Enqueue);
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
                    task.Branch(personalBest).ForEach(stack.Push);
                }
                else if (personalBest == null || task.CompareTo(personalBest) > 0)
                {
                    lock(GlobalBestLock)
                    {
                        personalBest = GlobalBest;
                        if (GlobalBest == null || task.CompareTo(GlobalBest) > 0)
                        {
                            GlobalBest = task;
                        }
                    }
                }
            } while (true);
        }
    }
}
