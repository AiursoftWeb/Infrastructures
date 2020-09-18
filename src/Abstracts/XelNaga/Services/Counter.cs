using Aiursoft.Scanner.Interfaces;

namespace Aiursoft.XelNaga.Services
{
    public class Counter : ISingletonDependency
    {
        private readonly object _obj = new object();

        public int GetUniqueNo()
        {
            lock (_obj)
            {
                return GetCurrent++;
            }
        }

        public int GetCurrent { get; private set; }
    }
}
