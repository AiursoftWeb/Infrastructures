using Aiursoft.Scanner.Interfaces;

namespace Aiursoft.XelNaga.Services
{
    public class Counter : ISingletonDependency
    {
        private readonly object _obj = new object();

        /// <summary>
        /// Get a new scope unique number which is one larger than current.
        /// </summary>
        /// <returns></returns>
        public int GetUniqueNo()
        {
            lock (_obj)
            {
                return ++GetCurrent;
            }
        }

        /// <summary>
        /// Last returned counter value. If a initial counter, will be -1.
        /// </summary>
        public int GetCurrent { get; private set; } = -1;
    }
}
