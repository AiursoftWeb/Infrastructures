using Aiursoft.Pylon.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class ViewAuditLogAddressModel : UserOperationAddressModel, IPageable
    {
        [Range(1, 100)]
        /// <summary>
        /// Default is 10
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Starts from 0.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 0;
    }
}
