using System.Net;

namespace Aiursoft.Handler.Models
{
    public class AiurProtocol
    {
        public virtual ErrorType Code { get; set; }
        public virtual string Message { get; set; }

        public HttpStatusCode ConvertToHttpStatusCode()
        {
            switch (Code)
            {
                case ErrorType.Success:
                    return HttpStatusCode.OK;
                case ErrorType.WrongKey:
                case ErrorType.Unauthorized:
                    return HttpStatusCode.Unauthorized;
                case ErrorType.InsufficientPermissions:
                    return HttpStatusCode.Forbidden;
                case ErrorType.Gone:
                    return HttpStatusCode.Gone;
                case ErrorType.NotFound:
                    return HttpStatusCode.NotFound;
                case ErrorType.UnknownError:
                    return HttpStatusCode.InternalServerError;
                case ErrorType.HasSuccessAlready:
                    return HttpStatusCode.AlreadyReported;
                case ErrorType.Conflict:
                    return HttpStatusCode.Conflict;
                case ErrorType.InvalidInput:
                    return HttpStatusCode.BadRequest;
                case ErrorType.Timeout:
                    return HttpStatusCode.RequestTimeout;
                case ErrorType.TooManyRequests:
                    return HttpStatusCode.TooManyRequests;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}
