using System.Collections.Generic;

namespace Aiursoft.WWW.Models
{
    public class Navbar
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public List<Navbar> Dropdowns { get; set; } = new List<Navbar>();
    }
}
