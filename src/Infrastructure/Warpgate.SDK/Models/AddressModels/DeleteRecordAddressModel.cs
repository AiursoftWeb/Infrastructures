using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels;

public class DeleteRecordAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required] [ValidDomainName] public string RecordName { get; set; }
}