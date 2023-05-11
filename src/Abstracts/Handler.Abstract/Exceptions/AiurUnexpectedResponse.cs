using System;
using Aiursoft.Handler.Models;

namespace Aiursoft.Handler.Exceptions;

/// <summary>
///     Throw this exception if the json respond by the Aiursoft Server was not with code = 0.
///     Catch it in your own code or just use `AiurExpHandler`.
/// </summary>
public class AiurUnexpectedResponse : Exception
{
    public AiurUnexpectedResponse(AiurProtocol response) : base(response.Message)
    {
        Response = response;
    }

    public AiurProtocol Response { get; set; }
    public ErrorType Code => Response.Code;
}