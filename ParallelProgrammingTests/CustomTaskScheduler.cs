using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgrammingTests
{
    public class CustomTaskScheduler : TaskScheduler
    {
        private Queue<Task> queue = new Queue<Task>();

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return queue;
        }

        protected override void QueueTask(Task task)
        {
            queue.Enqueue(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            Console.WriteLine("Try execute inline");
            if (!base.TryExecuteTask(task))
                return false;
            else
                if (taskWasPreviouslyQueued)
                    queue.Dequeue();
            while (queue.Count > 0)
            {
                Task t = queue.Peek();
                if (base.TryExecuteTask(t))
                {
                    queue.Dequeue();
                    Thread.Sleep(10); 
                }
                
            }

            return true;
        }

    }
}
