using System;

namespace Aiursoft.DocGenerator.Attributes
{
    public class APIProduces : Attribute
    {
        public Type PossibleType;
        public APIProduces(Type type)
        {
            PossibleType = type;
        }
    }
}
