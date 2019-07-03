using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

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
