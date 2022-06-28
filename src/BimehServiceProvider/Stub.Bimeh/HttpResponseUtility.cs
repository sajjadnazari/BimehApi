using Common.Bimeh.DTos.Common;
using System.Net;

namespace Stub.Bimeh
{
    public static class HttpResponseUtility
    {
        public static ServiceResult<string> StatusCodeHandler(int statusCode)
        {
            var sr = new ServiceResult<string>();

            switch (statusCode)
            {
                case (int)HttpStatusCode.Moved:
                    sr.AddResult("The requested page has been permanently moved to another location.", true);
                    break;
                case (int)HttpStatusCode.Redirect:
                    sr.AddResult("The address of requested resource has been changed temporarily.", true);
                    break;
                case (int)HttpStatusCode.SeeOther:
                    sr.AddResult("The response to the request can be found under another address.", true);
                    break;
                case (int)HttpStatusCode.NotModified:
                    sr.AddResult("The response has not been modified.", true);
                    break;
                case (int)HttpStatusCode.BadRequest:
                    sr.AddResult("The server could not understand the request due to invalid syntax.", true);
                    break;
                case (int)HttpStatusCode.Unauthorized:
                    sr.AddResult("Authentication is required.", true);
                    break;
                case (int)HttpStatusCode.Forbidden:
                    sr.AddResult("You do not have access to this section", true);
                    break;
                case (int)HttpStatusCode.NotFound:
                    sr.AddResult("The server can not find the requested resource..", true);
                    break;
                case (int)HttpStatusCode.MethodNotAllowed:
                    sr.AddResult("A request method is not supported for the requested resource.", true);
                    break;
                case (int)HttpStatusCode.RequestTimeout:
                    sr.AddResult("The server timed out waiting for the request", true);
                    break;
                case (int)HttpStatusCode.LengthRequired:
                    sr.AddResult("Server rejected the request because the Content-Length header field is not defined and the server requires it.", true);
                    break;
                case (int)HttpStatusCode.RequestEntityTooLarge:
                    sr.AddResult("The request is larger than the server is willing or able to process. ( A large file may be selected. ).", true);
                    break;
                case (int)HttpStatusCode.RequestUriTooLong:
                    sr.AddResult("The address requested for processing by the server is too long.", true);
                    break;
                case (int)HttpStatusCode.UnsupportedMediaType:
                    sr.AddResult("The media format of the requested data is not supported by the server.", true);
                    break;
                case (int)HttpStatusCode.InternalServerError:
                    sr.AddResult("The server encountered an error and was unable to execute the request.", true);
                    break;
                case (int)HttpStatusCode.NotImplemented:
                    sr.AddResult("The request method is not supported by the server and cannot be handled.", true);
                    break;
                case (int)HttpStatusCode.BadGateway:
                    sr.AddResult("The server, while working as a gateway to get a response needed to handle the request.", true);
                    break;
                case (int)HttpStatusCode.ServiceUnavailable:
                    sr.AddResult("The server is currently unavailable.", true);
                    break;
                default:
                    break;
            };

            return sr;
        }
    }
}
