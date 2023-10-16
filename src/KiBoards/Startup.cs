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
                var kibanaUri = new Uri(Environment.GetEnvironmentVariable("KIB_KIBANA_HOST") ?? "http://localhost:5601");
                var kibanaClient = new KiBoardsKibanaClient(kibanaUri, httpClient);

                messageSink.WriteMessage($"Waiting for Kibana {kibanaUri}");

                while (true)
                {
                    try
                    {
                        var response = await kibanaClient.GetStatus(CancellationToken.None);

                        string level = response?.Status?.Overall?.Level ?? throw new Exception("Kibana status is not available.");

                        if (level != "available")
                            throw new Exception("Kibana not available.");

                        messageSink.WriteMessage($"Kibana status: {level}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        messageSink.WriteMessage(ex.Message);
                        await Task.Delay(5000);
                    }
                }

                var ndjsonFiles =  Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ndjson");

                messageSink.WriteMessage($"Found {ndjsonFiles.Length} ndjson file(s)");

                foreach (var ndjsonFile in ndjsonFiles) {
                    WriteMessage(messageSink, $"Imporing {ndjsonFile}");
                    var results = await kibanaClient.ImportSavedObjectsAsync(ndjsonFile);
                    WriteMessage(messageSink, $"Import {results.Success} {results.SuccessCount}");
                }
            });            
        }
    }
}
