using System.ComponentModel.DataAnnotations;

namespace Aiursoft.XelNaga.Attributes
{
    public class CSVProperty : ValidationAttribute
    {
        public string Name;
        public CSVProperty(string name)
        {
            Name = name;
        }
    }
}
