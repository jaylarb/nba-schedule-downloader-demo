namespace Nba.Business.Interface
{
    /// <summary>
    /// Created this interface to facilitate mocking HttpClient in tests.
    /// </summary>
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
