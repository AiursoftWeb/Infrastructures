using Aiursoft.Scanner.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Models
{
    public class DemoService : ITransientDependency
    {
        public static bool Done = false;
        public static bool DoneAsync = false;

        public void DoSomethingSlow()
        {
            Done = false;
            Thread.Sleep(200);
            Done = true;
        }

        public async Task DoSomethingSlowAsync()
        {
            DoneAsync = false;
            await Task.Delay(200);
            DoneAsync = true;
        }
    }
}
