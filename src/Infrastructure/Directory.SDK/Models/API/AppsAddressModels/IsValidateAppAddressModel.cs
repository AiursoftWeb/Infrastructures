using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class IsValidateAppAddressModel
{
    [Required] [IsGuidOrEmpty] public virtual string AppId { get; set; }

    [Required] public virtual string AppSecret { get; set; }
}