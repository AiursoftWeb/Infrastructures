using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.APIAddressModels
{
    public class AllUserGrantedAddressModel
    {
        /// <summary>
        /// Default is 10
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Starts from 1.
        /// </summary>
        public int PageNumber { get; set; } = 0;

        [Required]
        public string AccessToken { get; set; }
    }
}
