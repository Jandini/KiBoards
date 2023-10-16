using KiBoards.Services;

[assembly: TestStartup("KiBoards.Startup")]

namespace KiBoards
{
    public class Startup
    {
        public Startup()
        {
            Task.Factory.StartNew(async () =>
            {
                var httpClient = new HttpClient();
                var kibanaClient = new KibanaClient(httpClient, new Uri("http://localhost:5601"));
                
                await kibanaClient.WaitForKibanaAsync(CancellationToken.None);
            });
        }
    }
}
