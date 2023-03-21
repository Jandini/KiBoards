namespace KiBoards.Services
{
    public class KibanaDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("kbn-xsrf", "true");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
