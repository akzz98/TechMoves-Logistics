namespace TechMoves_Logistics.Services
{
    public static class ApiHttpResponseExtensions
    {
        // Throws ApiClientException when the API returns a non-success status code.
        public static async Task EnsureApiSuccessAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var message = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(message))
                message = response.ReasonPhrase ?? "The API request failed.";
            else
                message = message.Trim('"');

            throw new ApiClientException(response.StatusCode, message);
        }
    }
}
