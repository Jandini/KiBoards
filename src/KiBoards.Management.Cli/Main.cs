using KiBoards.Management;
using KiBoards.Management.Models.Spaces;
using Microsoft.Extensions.Logging;

internal class Main(ILogger<Main> logger, KibanaHttpClient kibanaHttpClient) 
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Making kibana dark");
        await kibanaHttpClient.SetDarkModeAsync(false, null, cancellationToken);

        var space = new KibanaSpace()
        {
            Id = "default",
            Name = "Krowa",
            Initials = "D",
            Color = "#FFDD00",
            Description = "Hello world",
            DisabledFeatures = []
        };


        logger.LogInformation($"Getting default space");
        //space = await kibanaHttpClient.GetSpaceAsync("default", cancellationToken);

        logger.LogInformation("Space: {@defaultSpace}", space);




        logger.LogInformation("Updating default Kibana space {@space}", space);
        var response = await kibanaHttpClient.UpdateSpaceAsync(space, cancellationToken);

        logger.LogInformation("Space update: {status}", response.StatusCode);

        
        
    }
}
