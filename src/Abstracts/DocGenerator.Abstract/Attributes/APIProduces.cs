using System;

namespace Aiursoft.DocGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class APIProduces : Attribute
    {
        public Type PossibleType;
        public APIProduces(Type type)
        {
            PossibleType = type;
        }
    }
}
