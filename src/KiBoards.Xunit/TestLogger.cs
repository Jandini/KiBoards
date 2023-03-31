using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace KiBoards.Xunit.Logger
{
    [FriendlyName("kiboards")]
    [ExtensionUri("logger://Microsoft/TestPlatform/XunitKiBoardsLogger/v1")]
    public class TestLogger : ITestLoggerWithParameters
    {
        static TestLogger()
        {
            Console.WriteLine($"Logging with KiBoards {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
        }

        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            events.TestRunComplete += TestRunComplete;
        }

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            events.TestRunComplete += TestRunComplete;
        }

        private void TestRunComplete(object sender, TestRunCompleteEventArgs e)
        {
            Console.WriteLine($"ElapsedTimeInRunningTests: {e.ElapsedTimeInRunningTests}");
        }
    }
}
