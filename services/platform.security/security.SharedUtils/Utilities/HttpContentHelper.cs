using Newtonsoft.Json;
using System.Text;

namespace SharedLibrary.Utilities
{
    /// <summary>
    /// Convert the rolePayload object to a JSON string and create an HttpContent
    ///  with the Content-Type (application/json) for the HTTP request.
    /// </summary>
    public static class HttpContentHelper
    {
        public static HttpContent CreateHttpContent(object payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
