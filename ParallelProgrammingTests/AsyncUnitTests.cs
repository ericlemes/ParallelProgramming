using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using ParallelProgrammingTests;

namespace ParallelProgrammingTests
{
    [TestClass]
    public class ParallelProgrammingTests
    {
        [TestMethod]
        public void TestConcurrencyWithSingleThread()        
        {            
            CustomTaskScheduler scheduler = new CustomTaskScheduler();            

            TaskFactory factory = new TaskFactory(scheduler);            
            Task t1 = factory.StartNew(() =>
            {
                WriteALotOfLines("file1", 500, factory);                
            });            
            Task t2 = factory.StartNew(() =>
            {
                WriteALotOfLines("file2", 500, factory);                                
            });

            t1.Wait();
            t2.Wait();            
        }

        private static void WriteALotOfLines(string fileName, int lineCount, TaskFactory factory)
        {
            Console.WriteLine("Started writing file " + fileName);
            WriteContext ctx = new WriteContext();
            ctx.FileStream = new FileStream(fileName, FileMode.Create);
            ctx.Count = 0;
            ctx.Factory = factory;
            ctx.MaxCount = lineCount;
            ctx.FileName = fileName;
            byte[] bytes = Encoding.UTF8.GetBytes("Linha " + ctx.Count.ToString() + " Thread ID: " + Thread.CurrentThread.ManagedThreadId.ToString());
            Task t = factory.FromAsync<byte[], int, int>(ctx.FileStream.BeginWrite, ctx.FileStream.EndWrite, bytes, 0, bytes.Length, ctx);
            t.ContinueWith(WriteOneMoreLine);
        }
        
        private static void WriteOneMoreLine(Task task)
        {
            WriteContext ctx = (WriteContext)task.AsyncState;
            ctx.Count++;
            if (ctx.Count >= ctx.MaxCount)
            {
                ctx.FileStream.Close();
                return;
            }
            Console.WriteLine("Writing " + ctx.FileName + ": " + ctx.Count.ToString() + " Thread ID: " + Thread.CurrentThread.ManagedThreadId.ToString());
            byte[] bytes = Encoding.UTF8.GetBytes("Linha " + ctx.Count.ToString());
            Task t = 
                ctx.Factory.FromAsync<byte[], int, int>(ctx.FileStream.BeginWrite, ctx.FileStream.EndWrite, bytes, 0, bytes.Length, ctx);
            t.ContinueWith(WriteOneMoreLine);            
        }

        public class WriteContext
        {
            public TaskFactory Factory
            {
                get;
                set;
            }

            public FileStream FileStream
            {
                get;
                set;
            }

            public int Count
            {
                get;
                set;
            }

            public int MaxCount
            {
                get;
                set;
            }

            public string FileName
            {
                get;
                set;
            }
        }

        [TestMethod]
        public void TestConcurrencyWithDefaultTaskScheduler()
        {
            Task t1 = Task.Factory.StartNew(() =>
            {
                WriteALotOfLines("file1", 500, Task.Factory);
            });
            Task t2 = Task.Factory.StartNew(() =>
            {
                WriteALotOfLines("file2", 500, Task.Factory);
            });

            t1.Wait();
            t2.Wait();
        }        
        
    }
}
