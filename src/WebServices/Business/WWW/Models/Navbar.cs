using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Models
{
    public class Navbar
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public List<Navbar> Dropdowns { get; set; } = new List<Navbar>();
    }
}
