using Aiursoft.Pylon.Interfaces;

namespace Aiursoft.Pylon.Services
{
    public class Counter : ISingletonDependency
    {
        private readonly object _obj = new object();
        public int _Counted = 0;
        public int GetUniqueNo
        {
            get
            {
                lock (_obj)
                {
                    _Counted++;
                }
                return _Counted;
            }
        }

        public int GetCurrent
        {
            get
            {
                return _Counted;
            }
        }
    }
}
