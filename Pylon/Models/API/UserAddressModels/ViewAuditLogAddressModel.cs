namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class ViewAuditLogAddressModel : UserOperationAddressModel
    {
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Starts from 0.
        /// </summary>
        public int PageNumber { get; set; } = 0;
    }
}
