namespace TechMoves_Logistics.Services
{
    // Thrown when an API call returns a non-success HTTP status code.
    public class ApiClientException : Exception
    {
        public ApiClientException(System.Net.HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public System.Net.HttpStatusCode StatusCode { get; }
    }
}
