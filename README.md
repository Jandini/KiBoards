# KiBoards

[![Build](https://github.com/Jandini/KiBoards/actions/workflows/build.yml/badge.svg)](https://github.com/Jandini/KiBoards/actions/workflows/build.yml)
[![NuGet](https://github.com/Jandini/KiBoards/actions/workflows/nuget.yml/badge.svg)](https://github.com/Jandini/KiBoards/actions/workflows/nuget.yml)

Provides visualization of unit test results using Elasticsearch and Kibana for Xunit test framework.

# Quick Start

In just a few simple steps, you can have your unit test results stored in Elasticsearch.

* Create a new Xunit test project.

* Add the KiBoards NuGet package to the project.

* Include the Xunit test framework attribute  `[assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]` in your project. Place the attribute  only once, in any class, right after the `using` statements and before the `namespace` declaration.

* The initial unit test class will resemble this:

```c#
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


Now, after your unit tests are executed, the results are saved to the Elasticsearch host defined by the `KIB_ELASTICSEARCH_HOST` variable, with a default value of [http://localhost:9200](http://localhost:9200/).

You can specify `KIB_KIBANA_HOST` to automatically build Kibana dashboards like in the example below [http://10.120.235.20:5601]: 

![kiboards](https://github.com/user-attachments/assets/9a908bc0-a700-49dc-8f4a-d2257eff1fe6)

The dashboards are created in separate Kibana's space called KiBoards. 


Following code demonstrate how to use Restore customized dashboards, create test startup class that executes constructor before tests run.

```C#
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
        // Add variable that will be indexed as "variables.VERSION" into elasticsearch
        Environment.SetEnvironmentVariable("KIB_VAR_VERSION", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);            
    }     
}
```


---
Created from [JandaBox](https://github.com/Jandini/JandaBox)
