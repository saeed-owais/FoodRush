using System.Net;

namespace FoodRush.Infrastructure.Notifications;

public sealed class EmailSendFailedException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public EmailSendFailedException(
        HttpStatusCode statusCode,
        string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}