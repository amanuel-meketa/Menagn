namespace security.business.Contracts
{
    public interface IIdentityService
    {
        public Task<string> GetAccessTokenAsync();
        public Task<string> GetClientIdAsync(string accessToken);
        public Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string? accessToken = null, HttpContent? content = null);
    }
}
