using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgrammingTests
{
    [TestClass]
    public class ThreadWithLocks
    {
        private static readonly int bubbleSize = 1000;
        private static readonly int numberOfJobs = 400;

        private void DequeueAndProcess(Queue<Job> queue)
        {
            while (true)
            {
                Job job = null;
                lock (queue)
                {
                    if (queue.Count > 0)
                        job = queue.Dequeue();

                    if (job == null)
                    {
                        Thread.Sleep(0);
                        continue;
                    }
                    job.JobDelegate();
                    job.JobEndedNotifier.Set();
                }
            }
        }

        private void CreateJobsAndProcessInParallel(int numberOfThreads)
        {
            ConcurrentBag<int> threadIDs = new ConcurrentBag<int>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Queue<Job> queue = new Queue<Job>();
            List<Thread> list = new List<Thread>();
            List<ManualResetEvent> eventList = new List<ManualResetEvent>();
            for (int i = 0; i < numberOfThreads; i++)
            {
                Thread t = new Thread(() => { DequeueAndProcess(queue); });
                list.Add(t);
                t.Start();
            }
            for (int i = 0; i < numberOfJobs; i++)
            {
                ManualResetEvent e = new ManualResetEvent(false);
                eventList.Add(e);
                ThreadStart t = new ThreadStart(() =>
                {
                    BubbleSort.DoBigBubbleSort(bubbleSize, threadIDs);
                });
                queue.Enqueue(new Job(t, e));
            }
            foreach (ManualResetEvent e in eventList)
                e.WaitOne();
            foreach (Thread t in list)
                t.Abort();

            watch.Stop();
            Console.WriteLine("Total elapsed (ms): " + watch.ElapsedMilliseconds);
        }

        [TestMethod]
        public void SingleThreadTest()
        {
            CreateJobsAndProcessInParallel(1);
        }

        [TestMethod]
        public void TwoThreadsTest()
        {
            CreateJobsAndProcessInParallel(2);
        }

        [TestMethod]
        public void FourThreadsTest()
        {
            CreateJobsAndProcessInParallel(4);
        }

        [TestMethod]
        public void HundredhreadsTest()
        {
            CreateJobsAndProcessInParallel(100);
        }

        [TestMethod]
        public void TwoHundredhreadsTest()
        {
            CreateJobsAndProcessInParallel(200);
        }

        [TestMethod]
        public void FourHundredhreadsTest()
        {
            CreateJobsAndProcessInParallel(400);
        }

        [TestMethod]
        public void TaskBasedTest()
        {
            ConcurrentBag<int> threadIDs = new ConcurrentBag<int>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Task> l = new List<Task>();
            for (int i = 0; i < numberOfJobs; i++)
                l.Add(Task.Factory.StartNew(() => { BubbleSort.DoBigBubbleSort(bubbleSize, threadIDs); }));
            foreach (Task t in l)
                t.Wait();
            watch.Stop();
            Console.WriteLine("Total elapsed (ms): " + watch.ElapsedMilliseconds);
            Console.WriteLine("Number of threads used: " + threadIDs.Distinct().Count());
        }
    }
}
