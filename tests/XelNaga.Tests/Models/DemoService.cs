using Aiursoft.Scanner.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Models
{
    public class DemoService : ITransientDependency
    {
        public static bool Done;
        public static bool DoneAsync;

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
