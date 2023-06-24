using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.XelNaga.Attributes;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels;

public class CreateNewRecordAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required]
    [ValidDomainName]
    [MaxLength(50)]
    [MinLength(5)]
    public string NewRecordName { get; set; }

    [Required] public RecordType Type { get; set; }

    [Required]
    [MaxLength(1000)]
    [MinLength(5)]
    [Url]
    public string TargetUrl { get; set; }

    [Required] public bool Enabled { get; set; }

    public string Tags { get; set; }
}