using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Aiursoft.Pylon.Attributes
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
