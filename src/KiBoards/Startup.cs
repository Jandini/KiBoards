using KiBoards.Management;
using KiBoards.Management.Models.Spaces;
using System.Net;
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
                    var kibanaUri = new Uri(Environment.GetEnvironmentVariable("KIB_KIBANA_HOST") ?? "http://localhost:5601");

                    var httpClient = new HttpClient()
                    {
                        BaseAddress = kibanaUri,
                    };

                    var kibanaClient = new KibanaHttpClient(httpClient);

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
                    messageSink.WriteMessage($"Trying to create Kibana space for KiBoards...");

                    var kiboards = KibanaSpace.Create(
                        id: "kiboards",
                        name: GetEnvironmentVariable("KIB_SPACE_NAME", "KiBoards"),
                        initials: GetEnvironmentVariable("KIB_SPACE_INITIALS", "Ki"),
                        color: GetEnvironmentVariable("KIB_SPACE_COLOR", "#000000"),
                        disabledFeatures: GetEnvironmentVariable("KIB_DISABLE_FEATURES", "discover,enterpriseSearch,logs,infrastructure,apm,uptime,observabilityCases,slo,siem,securitySolutionCases,canvas,maps,ml,visualize,dev_tools,advancedSettings,indexPatterns,filesManagement,filesSharedImage,savedObjectsManagement,savedObjectsTagging,osquery,actions,generalCases,guidedOnboardingFeature,rulesSettings,maintenanceWindow,stackAlerts,fleetv2,fleet,monitoring", true),
                        imageUrl: "",
                        description: GetEnvironmentVariable("KIB_SPACE_DESCRIPTION", "KiBoards dashboards")
                    );

                    var result = await kibanaClient.CreateSpaceAsync(kiboards);

                    if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        messageSink.WriteMessage($"KiBoards space already exist. Updating the space...");
                        result = await kibanaClient.UpdateSpaceAsync(kiboards);
                    }

                    if (result.IsSuccessStatusCode)
                        messageSink.WriteMessage($"KiBoards space configured successfully.");
                    else
                        messageSink.WriteMessage($"KiBoards space failed to configure due to {result.ReasonPhrase}");

                    var defaultRoute = GetEnvironmentVariable("KIB_DEFAULT_ROUTE", "/app/dashboards");
                    messageSink.WriteMessage($"Configuring default route to {defaultRoute}");
                    result = await kibanaClient.SetDefaultRoute(defaultRoute, kiboards.Id, CancellationToken.None);

                    if (result.IsSuccessStatusCode)
                        messageSink.WriteMessage($"KiBoards default route configured successfully.");
                    else
                        messageSink.WriteMessage($"KiBoards default route configuration failed due to {result.ReasonPhrase}.");


                    var darkModeVariable = GetEnvironmentVariable("KIB_DARK_MODE", "0");
                    var darkMode = darkModeVariable == "1" || darkModeVariable.ToLower() == "true";
                    messageSink.WriteMessage($"{(darkMode ? "Enabling" : "Disabling")} Kibana dark mode.");
                    await kibanaClient.SetDarkModeAsync(darkMode, KibanaSpace.KiBoards.Id, CancellationToken.None);

                    var ndjsonFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), attribute.SearchPattern);

                    messageSink.WriteMessage($"Found {ndjsonFiles.Length} ndjson file(s)");

                    foreach (var ndjsonFile in ndjsonFiles.OrderBy(a => a))
                    {
                        string fileName = Path.GetFileName(ndjsonFile);

                        messageSink.WriteMessage($"Importing {fileName}");

                        var spaceId = fileName == "KiBoards.ndjson" ? "kiboards" : null;

                        var results = await kibanaClient.ImportSavedObjectsAsync(ndjsonFile, attribute.Overwrite, spaceId);
                        messageSink.WriteMessage($"Imported {results.SuccessCount} object(s)");

                        if (!results.Success && results.SuccessCount > 0)
                            messageSink.WriteMessage("Warning: Some objects were not imported. Please ensure proper import order based on their dependencies.");
                    }
                });
            }
        }

        private string GetEnvironmentVariable(string name, string defaultValue = null, bool allowEmpty = false)
        {
            var value = Environment.GetEnvironmentVariable(name);

            if (value == null)
                return defaultValue;

            if (!allowEmpty && string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value;
        }
    }
}
