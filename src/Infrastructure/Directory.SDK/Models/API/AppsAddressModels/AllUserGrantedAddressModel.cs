using Aiursoft.AiurProtocol.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class AllUserGrantedAddressModel : IPager
{
    // TODO: Client side attribute to limit the access token.
    [Required] public string AccessToken { get; set; }

    /// <summary>
    ///     Default is 10
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    ///     Starts from 1.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
}