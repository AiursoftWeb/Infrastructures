using Aiursoft.Scanner.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Models
{
    public class DemoService : ITransientDependency
    {
        public static bool Done;
        public static bool DoneAsync;
        public static int DoneTimes = 0;
        private static object obj = new object();

        public void DoSomethingSlow()
        {
            Done = false;
            Thread.Sleep(200);
            Done = true;
        }

        public async Task DoSomethingSlowAsync()
        {
            Console.WriteLine("\a");
            DoneAsync = false;
            await Task.Delay(200);
            DoneAsync = true;
            lock (obj)
            {
                DoneTimes++;
            }
        }
    }
}
