using System.ComponentModel.DataAnnotations;
using Aiursoft.XelNaga.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class IsValidateAppAddressModel
{
    [Required] [IsGuidOrEmpty] public virtual string AppId { get; set; }

    [Required] [IsGuidOrEmpty] public virtual string AppSecret { get; set; }
}