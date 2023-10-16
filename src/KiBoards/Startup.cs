using KiBoards.Services;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestStartup("KiBoards.Startup")]

namespace KiBoards
{
    public class Startup
    {
        static void WriteMessage(IMessageSink messageSink, string message)
        {
            messageSink.OnMessage(new DiagnosticMessage(message));
        }

        public Startup(IMessageSink messageSink)
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var httpClient = new HttpClient();
                var kibanaHost = Environment.GetEnvironmentVariable("KIB_KIBANA_HOST") ?? "http://localhost:5601";
                var kibanaClient = new KibanaClient(httpClient, new Uri(kibanaHost));

                WriteMessage(messageSink, $"Waiting for Kibana {kibanaHost}");

                await kibanaClient.WaitForKibanaAsync(CancellationToken.None);
                await kibanaClient.SetDarkModeAsync(true, CancellationToken.None);                
            });
            
        }
    }
}
