using System;
using Aiursoft.Handler.Models;

namespace Aiursoft.Gateway.SDK.Models.API.HomeViewModels;

public class IndexViewModel : AiurProtocol
{
    public DateTime ServerTime { get; set; }
    public bool SignedIn { get; set; }
    public string Local { get; set; }
    public string Exponent { get; set; }
    public string Modulus { get; set; }
}