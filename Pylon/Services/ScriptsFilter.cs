using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Services
{
    public class ScriptsFilter
    {
        public string Filt(string html)
        {
            return html.Replace("<scripts", "< scripts");
        }
    }
}
