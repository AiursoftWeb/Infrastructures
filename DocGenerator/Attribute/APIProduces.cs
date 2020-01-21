using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Aiursoft.DocGenerator.Attribute
{
    public class APIProduces : ActionFilterAttribute
    {
        public Type PossibleType;
        public APIProduces(Type type)
        {
            PossibleType = type;
        }
    }
}
