using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
    public class IOTests
    {
        [TestMethod]
        public void SynchronousIOTest()
        {
            FileStream fs = new FileStream("file1.txt", FileMode.Create, FileAccess.ReadWrite);
            byte[] buffer = new byte[100 * 1024 * 1024]; //Não faça isso. Aloca 100Mb de memória.            
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
            fs.Close();
        }

        [TestMethod]
        public void AsyncIOTest()
        {
            FileStream fs = new FileStream("file1.txt", FileMode.Create, FileAccess.ReadWrite);
            byte[] buffer = new byte[100 * 1024 * 1024]; //Não faça isso. Aloca 100Mb de memória.            
            fs.BeginWrite(buffer, 0, buffer.Length, null, null);
            fs.Flush();
            fs.Close();
        }

        [TestMethod]
        public void BigSyncIOTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < 10; i++)
            {
                FileStream fs = new FileStream("file1.txt", FileMode.Create, FileAccess.ReadWrite);
                byte[] buffer = new byte[100 * 1024 * 1024]; //Não faça isso. Aloca 100Mb de memória.            
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();

                BubbleSort.DoBigBubbleSort(10000, null);
            }

            watch.Stop();
            Console.WriteLine("Total elapsed (ms): " + watch.ElapsedMilliseconds);
        }

        private ManualResetEvent ev;

        [TestMethod]
        public void BigAsyncIOTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            ev = new ManualResetEvent(false);

            WriteFinishedCallback(null);
            ev.WaitOne();

            watch.Stop();
            Console.WriteLine("Total elapsed (ms): " + watch.ElapsedMilliseconds);
        }

        public void WriteFinishedCallback(IAsyncResult ar)
        {
            State state = null;
            if (ar == null)
            {
                state = new State();
                state.Current = 0;
            }
            else
            {
                state = (State)ar.AsyncState;
                state.FileStream.EndWrite(ar);
                state.FileStream.Flush();
                state.FileStream.Close();
                state.Current++;
                if (state.Current == 10)
                {
                    ev.Set();
                    return;
                }
            }

            FileStream fs = new FileStream("file1.txt", FileMode.Create, FileAccess.ReadWrite);
            byte[] buffer = new byte[100 * 1024 * 1024]; //Não faça isso. Aloca 100Mb de memória.            
            fs.BeginWrite(buffer, 0, buffer.Length, WriteFinishedCallback, state);
            state.FileStream = fs;

            BubbleSort.DoBigBubbleSort(10000, null);

        }

        [TestMethod]
        public void BigAsyncIOTest2()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            for (int i = 0; i < 10; i++)            
                DoBigWriteAsync().Wait();
            
            watch.Stop();
            Console.WriteLine("Total elapsed (ms): " + watch.ElapsedMilliseconds);
        }

        private async Task DoBigWriteAsync()
        {
            FileStream fs = new FileStream("file1.txt", FileMode.Create, FileAccess.ReadWrite);
            byte[] buffer = new byte[100 * 1024 * 1024]; //Não faça isso. Aloca 100Mb de memória.                        
            Task t = fs.WriteAsync(buffer, 0, buffer.Length);           
            Task t2 = t.ContinueWith(new Action<Task>((task) => { 
                fs.Flush();
                fs.Close();
            }));
            BubbleSort.DoBigBubbleSort(10000, null);
            await t;            
        }

    }

    public class State
    {
        public int Current
        {
            get;
            set;
        }

        public FileStream FileStream
        {
            get;
            set;
        }
    }
}
