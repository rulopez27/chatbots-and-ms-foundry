using System.Net;

namespace Roboto.Sdk.Exceptions
{
    /// <summary>
    /// Exception thrown when the Roboto API returns an error
    /// </summary>
    public class RobotoApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? ResponseContent { get; }

        public RobotoApiException(
            string message, 
            HttpStatusCode statusCode, 
            string? responseContent = null) 
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        public RobotoApiException(
            string message, 
            HttpStatusCode statusCode, 
            Exception innerException) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    public class RobotoAuthenticationException : RobotoApiException
    {
        public RobotoAuthenticationException(string message, string? responseContent = null)
            : base(message, HttpStatusCode.Unauthorized, responseContent)
        {
        }
    }

    public class RobotoNotFoundException : RobotoApiException
    {
        public RobotoNotFoundException(string message, string? responseContent = null)
            : base(message, HttpStatusCode.NotFound, responseContent)
        {
        }
    }

    public class RobotoValidationException : RobotoApiException
    {
        public RobotoValidationException(string message, string? responseContent = null)
            : base(message, HttpStatusCode.BadRequest, responseContent)
        {
        }
    }
}
