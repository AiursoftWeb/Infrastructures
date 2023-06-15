using System;

namespace Aiursoft.Portal.Models.ToolsViewModels;

public class UuidViewModel
{
    public string Format { get; set; } = "D";
    public string DefaultUuid { get; set; } = Guid.NewGuid().ToString("D");
}