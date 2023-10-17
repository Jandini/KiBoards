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



---
Created from [JandaBox](https://github.com/Jandini/JandaBox)
