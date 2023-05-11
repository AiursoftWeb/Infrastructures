using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels;

public class DeleteRecordAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] [ValidDomainName] public string RecordName { get; set; }
}