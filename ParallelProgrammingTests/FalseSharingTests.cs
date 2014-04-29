using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgrammingTests
{
    [TestClass]
    public class FalseSharingTests
    {
        private int bubbleSize = 20000;

        [TestMethod]
        public void BigBubbleSortTest()
        {
            BubbleSort.DoBigBubbleSort2(bubbleSize, null);
            BubbleSort.DoBigBubbleSort2(bubbleSize, null);
        }

        [TestMethod]
        public void BigBubbleSort2Test()
        {
            Queue<ManualResetEvent> q = new Queue<ManualResetEvent>();            
            for (int i = 0; i <= 1; i++)
            {
                ManualResetEvent e = new ManualResetEvent(false);
                Thread t = new Thread(() => {
                    BubbleSort.DoBigBubbleSort2(bubbleSize, null);
                    e.Set();
                });
                t.Start();
                q.Enqueue(e);
            }
            while (q.Count > 0)
                q.Dequeue().WaitOne();
        }
    }
}
