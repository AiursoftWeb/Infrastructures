using Aiursoft.Handler.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.API.APIAddressModels
{
    public class AllUserGrantedAddressModel : IPageable
    {
        /// <summary>
        /// Default is 10
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Starts from 1.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Required]
        public string AccessToken { get; set; }
    }
}
