# KiBoards

[![Build](https://github.com/Jandini/KiBoards/actions/workflows/build.yml/badge.svg)](https://github.com/Jandini/KiBoards/actions/workflows/build.yml)
[![NuGet](https://github.com/Jandini/KiBoards/actions/workflows/nuget.yml/badge.svg)](https://github.com/Jandini/KiBoards/actions/workflows/nuget.yml)

Provides visualization of unit test results using Elasticsearch and Kibana for the xUnit test framework.

## Quick Start

In just a few simple steps, you can have your unit test results stored in Elasticsearch and visualized with Kibana dashboards.

1. **Create a new xUnit test project.**

2. **Add the KiBoards NuGet package to the project.**

3. **Configure the test framework:**
   - Include the xUnit test framework attribute:
     ```csharp
     [assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]
     ```
     Place this attribute once in your project, right after the `using` statements and before the `namespace` declaration.

4. **Simple Unit Test Example:**
     ```csharp
     [assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]

     namespace Tests
     {
         public class UnitTest1
         {
             [Fact]
             public void TestSomething()
             {
                 // Your test logic goes here
             }
         }
     }
     ```

5. **Test Results and Dashboard Generation:**
   - After your tests execute, the results are saved to an Elasticsearch host defined by the `KIB_ELASTICSEARCH_HOST` environment variable. The default is [http://localhost:9200](http://localhost:9200/).
   - Specify the `KIB_KIBANA_HOST` environment variable (default is `http://localhost:5601`) to automatically build Kibana dashboards. Dashboards are created in a separate Kibana space called **KiBoards**.

   ![kiboards](https://github.com/user-attachments/assets/9a908bc0-a700-49dc-8f4a-d2257eff1fe6)

## Customizing Your Kibana Space

KiBoards uses a test startup class to create and configure a Kibana space automatically. You can customize this space by setting a few environment variables. The example below demonstrates how to configure your own space by overriding defaults.

### Example: Custom Test Startup

```csharp
using System.Reflection;
using Xunit.Abstractions;

[assembly: KiboardsTestStartup("TestStartup.Startup")]
[assembly: KiBoardsSavedObjects()]
[assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]

namespace TestStartup;

public class Startup 
{
    public Startup(IMessageSink messageSink)
    {
        // Set a custom variable that will be indexed as "variables.VERSION" into Elasticsearch.
        Environment.SetEnvironmentVariable(
            "KIB_VAR_VERSION", 
            Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        );

        // Optionally, configure your own Kibana space by setting these environment variables:
        // - KIB_SPACE_ID: Specifies the ID of the Kibana space (default is "kiboards"). Must be all lowercase letters with no spaces.
        // - KIB_SPACE_NAME: Human-friendly name for the space (e.g., "My Space")
        // - KIB_SPACE_INITIALS: Specifies the initials for the space (default is "Ki")
        // - KIB_SPACE_COLOR: The color code for the space badge (default is "#000000")
        // - KIB_SPACE_DESCRIPTION: Description of the Kibana space (default is "KiBoards dashboards")
        //
        // For example, to create a space with ID "myspace", you can set:
        // Environment.SetEnvironmentVariable("KIB_SPACE_ID", "myspace");
        // Environment.SetEnvironmentVariable("KIB_SPACE_NAME", "My Custom Space");
    }     
}
```

## Environment Variables and Their Purpose

KiBoards uses several environment variables to configure its connection to Elasticsearch and Kibana, as well as to customize the appearance and behavior of the Kibana space. The table below summarizes these variables:

| Variable Name                  | Default Value           | Description |
|--------------------------------|--------------------------|-------------|
| `KIB_ELASTICSEARCH_HOST`       | `http://localhost:9200`  | The full URL of your Elasticsearch instance where test results are stored. |
| `KIB_KIBANA_HOST`              | `http://localhost:5601`  | The full URL of your Kibana instance used to build dashboards. |
| `KIB_KIBANA_ACCEPT_ANY_CERT`   | `false`                  | If set to `"1"` or `"true"`, accepts any SSL certificate (useful for local/test environments with self-signed certificates). |
| `KIB_SPACE_ID`                 | `kiboards`               | The **ID** of the Kibana space to be created or updated. Must be lowercase, no spaces. |
| `KIB_SPACE_NAME`               | `KiBoards`               | The **display name** for the Kibana space (can include spaces and uppercase letters). |
| `KIB_SPACE_INITIALS`           | `Ki`                     | The initials to display on the space badge within Kibana. |
| `KIB_SPACE_COLOR`              | `#000000`                | The color code for the space badge in Kibana. |
| `KIB_SPACE_DESCRIPTION`        | `KiBoards dashboards`    | A brief description of the Kibana space, shown in the Kibana UI. |
| `KIB_DISABLE_FEATURES`         | *(not set)*              | Comma-separated list of Kibana features to disable in the space (e.g., `ml,dev_tools,canvas`). |
| `KIB_DEFAULT_ROUTE`            | `/app/dashboards`        | The default route/path users are directed to when accessing the space. |
| `KIB_DARK_MODE`                | `0`                      | If set to `"1"` or `"true"`, enables dark mode in Kibana for the space. |

## How It Works

1. **Kibana Connection and Status Check:**
   - The system reads `KIB_KIBANA_HOST` to establish a connection with your Kibana instance.
   - It repeatedly checks the status of Kibana until it confirms that Kibana is “available.”

2. **Kibana Space Creation/Update:**
   - A Kibana space is created (or updated if it already exists) using the defined environment variables.
   - The space ID defaults to `KIB_SPACE_ID` (or `"kiboards"` if not set), and additional properties such as name, initials, color, and description are configured via their corresponding variables.

3. **Dashboard Configuration:**
   - After the space is configured, the system sets a default route (`KIB_DEFAULT_ROUTE`) and applies dark mode settings (`KIB_DARK_MODE`) as defined by the environment variables.
   - Test results and imported saved objects (in `.ndjson` format) are then pushed to the configured space.

4. **Custom Variables:**
   - You can also set custom variables (e.g., `KIB_VAR_VERSION`) which are indexed into Elasticsearch along with your test results.

By adjusting these environment variables, you can tailor KiBoards’ behavior to fit your specific testing and visualization requirements, including configuring a completely customized Kibana space.

---

This project was created using [JandaBox](https://github.com/Jandini/JandaBox).
