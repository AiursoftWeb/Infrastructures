using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API
{
    public class AiurUserEmail
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        public bool Validated { get; set; } = false;
    }
}
