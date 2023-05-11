﻿using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels;

public class DeleteAppAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] [IsGuidOrEmpty] public string AppId { get; set; }
}