namespace security.business.Contracts
{
    public interface IIdentityService
    {
        public Task<string> GetAccessToken();
        public Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string accessToken = null, HttpContent content = null);
    }
}
