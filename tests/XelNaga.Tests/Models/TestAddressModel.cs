using System;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.XelNaga.Tests.Models;

internal class TestAddressModel
{
    public string Question { get; set; }
    public int Count { get; set; }

    [FromQuery(Name = "emailAddress")] public string Email { get; set; }

    public string MyNull { get; set; }
    public DateTime CreateTime { get; set; }
}