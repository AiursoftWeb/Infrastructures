﻿using System.ComponentModel.DataAnnotations;
using Aiursoft.Handler.Interfaces;

namespace Aiursoft.Directory.SDK.Models.API.UserAddressModels;

public class ViewAuditLogAddressModel : UserOperationAddressModel, IPageable
{
    /// <summary>
    ///     Default is 10
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    ///     Starts from 0.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
}