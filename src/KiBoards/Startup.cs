using KiBoards.Models.Spaces;
using KiBoards.Services;
using System.Reflection;
using Xunit.Abstractions;

[assembly: KiboardsTestStartup("KiBoards.Startup")]

namespace KiBoards
{
    public class Startup
    {

        public Startup(IMessageSink messageSink)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttribute<KiBoardsSavedObjectsAttribute>() != null);

            foreach (var assembly in assemblies)
            {
                var attribute = assembly.GetCustomAttribute<KiBoardsSavedObjectsAttribute>();

                var task = Task.Run(async () =>
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

                    var result = await kibanaClient.TryCreateSpaceAsync(Space.KiBoards);

                    if (result.IsSuccessStatusCode)
                        messageSink.WriteMessage($"KiBoards space created successfully.");
                    else
                        messageSink.WriteMessage($"KiBoards space failed to create due to {result.ReasonPhrase}");

                        await kibanaClient.TrySetDefaultRoute("/app/dashboards", Space.KiBoards.Id, CancellationToken.None);

                    var darkModeVariable = Environment.GetEnvironmentVariable("KIB_DARK_MODE");

                    if (!string.IsNullOrEmpty(darkModeVariable))
                    {
                        var darkMode = darkModeVariable == "1" || darkModeVariable.ToLower() == "true";
                        messageSink.WriteMessage($"{(darkMode ? "Enabling" : "Disabling")} Kibana dark mode.");

                        await kibanaClient.SetDarkModeAsync(darkMode, Space.KiBoards.Id, CancellationToken.None);
                    }

                    var ndjsonFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), attribute.SearchPattern);

                    messageSink.WriteMessage($"Found {ndjsonFiles.Length} ndjson file(s)");

                    foreach (var ndjsonFile in ndjsonFiles.OrderBy(a => a))
                    {
                        string fileName = Path.GetFileName(ndjsonFile);

                        messageSink.WriteMessage($"Importing {fileName}");

                        var spaceId = fileName == "KiBoards.ndjson" ? "kiboards" : null;

                        var results = await kibanaClient.ImportSavedObjectsAsync(ndjsonFile, spaceId, attribute.Overwrite);
                        messageSink.WriteMessage($"Imported {results.SuccessCount} object(s)");

                        if (!results.Success && results.SuccessCount > 0)
                            messageSink.WriteMessage("Warning: Some objects were not imported. Please ensure proper import order based on their dependencies.");
                    }
                });
            }
        }
    }
}
