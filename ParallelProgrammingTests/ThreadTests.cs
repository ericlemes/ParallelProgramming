using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgrammingTests
{
    public class ThreadTests
    {        
        public void DoSomething()
        {
            //Algum processamento.
        }

        public void DoSomethingInParallel()
        {
            Thread t = new Thread(DoSomething);
            t.Start();
            DoSomething();
        }
    }
}
