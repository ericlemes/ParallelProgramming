using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgrammingTests
{
    public static class BubbleSort
    {
        public static void Sort(IList<int> list)
        {
            bool swap = true;
            while (swap)
            {
                swap = false;
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i] < list[i - 1])
                    {
                        int tmp = list[i];
                        list[i] = list[i - 1];
                        list[i - 1] = tmp;
                        swap = true;
                    }
                }
            }
        }        

        public static void DoBigBubbleSort(int bubblesize, ConcurrentBag<int> threadIDs )
        {
            List<int> l = new List<int>(bubblesize);

            for (int i = bubblesize; i >= 0; i--)
                l.Add(i);
            Sort(l);
            Console.WriteLine("Job done. ThreadID: " + Thread.CurrentThread.ManagedThreadId.ToString());
            if (threadIDs != null)
                threadIDs.Add(Thread.CurrentThread.ManagedThreadId);
        }

        public static void Sort2<T>(IList<T> list) where T : IComparable
        {
            bool swap = true;
            while (swap)
            {
                swap = false;
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].CompareTo(list[i - 1]) < 0)
                    {
                        T tmp = list[i];
                        list[i] = list[i - 1];
                        list[i - 1] = tmp;
                        swap = true;
                    }
                }
            }
        }


        public static void DoBigBubbleSort2(int bubblesize, ConcurrentBag<int> threadIDs)
        {
            List<int> l = new List<int>(bubblesize);

            for (int i = bubblesize; i >= 0; i--)
                l.Add(i);

            Sort2(l);
            Console.WriteLine("Job done. ThreadID: " + Thread.CurrentThread.ManagedThreadId.ToString());
            if (threadIDs != null)
                threadIDs.Add(Thread.CurrentThread.ManagedThreadId);
        }
    }
}
