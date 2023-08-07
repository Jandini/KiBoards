using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Framework
{
    internal class TestMessageBus : IMessageBus
    {
        private IMessageBus _messageBus;
        private IMessageSink _messageSink;

        public TestMessageBus(IMessageBus messageBus, IMessageSink messageSink)
        {
            _messageBus = messageBus;
            _messageSink = messageSink;
        }

        public void Dispose()
        {
            _messageBus.Dispose();
        }

        private void LogMessage(string message)
        {
            _messageSink.OnMessage(new DiagnosticMessage(message));
        }

        private void LogTestCase(string messageType, ITestCase testCase)
        {
            LogMessage($"{messageType}: TestCase.UniqueID: {testCase.UniqueID}");
            LogMessage($"{messageType}: TestCase.DisplayName: {testCase.DisplayName}");
            LogMessage($"{messageType}: TestCase.Traits.Count: {testCase.Traits.Count}");

            if (testCase.SourceInformation != null)
            {
                LogMessage($"{messageType}: TestCase.SourceInformation.FileName: {testCase.SourceInformation.FileName}");
                LogMessage($"{messageType}: TestCase.SourceInformation.LineNumber: {testCase.SourceInformation.LineNumber}");
            }
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            _messageSink.OnMessage(new DiagnosticMessage($"MessageBus: {message}"));
            
            HandleMessageSinkMessage(message);

            return _messageBus.QueueMessage(message);
        }

        public void HandleMessageSinkMessage(IMessageSinkMessage message)
        {
            switch (message)
            {
                case ITestMethodStarting testMethodStarting:
                    LogMessage($"TestMethodStarting: TestCollection.DisplayName: {testMethodStarting.TestCollection.DisplayName}");
                    LogMessage($"TestMethodStarting: TestCollection.UniqueID: {testMethodStarting.TestCollection.UniqueID}");
                    LogMessage($"TestMethodStarting: TestCollection.TestAssembly.Assembly.AssemblyPath: {testMethodStarting.TestCollection.TestAssembly.Assembly.AssemblyPath}");
                    LogMessage($"TestMethodStarting: TestCases.Count: {testMethodStarting.TestCases.Count()}");

                    LogMessage($"TestMethodStarting: TestClass.Class.Name: {testMethodStarting.TestClass.Class.Name}");
                    break;

                case ITestCaseStarting testCaseStarting:
                    LogTestCase("TestCaseStarting", testCaseStarting.TestCase);
                    break;

                case ITestStarting testStarting:
                    LogMessage($"TestStarting: Test.DisplayName: {testStarting.Test.DisplayName}");
                    LogTestCase("TestCaseStarting", testStarting.TestCase);
                    break;

                case ITestClassConstructionStarting classConstructionStarting:
                    // Code to handle Xunit.Sdk.TestClassConstructionStarting case
                    break;

                case ITestClassConstructionFinished classConstructionFinished:
                    // Code to handle Xunit.Sdk.TestClassConstructionFinished case
                    break;


                case ITestSkipped testSkipped:
                    // Code to handle Xunit.Sdk.TestSkipped case
                    break;

                case ITestPassed testPassed:
                    // Code to handle Xunit.Sdk.TestPassed case
                    break;

                case ITestFailed testFailed:
                    LogTestCase("TestFailed", testFailed.TestCase);
                    LogMessage($"TestFailed: ExecutionTime: {testFailed.ExecutionTime}");
                    LogMessage($"TestFailed: Messages: {string.Join('\n', testFailed.Messages)}");
                    LogMessage($"TestFailed: Output: {testFailed.Output}");
                    // Code to handle Xunit.Sdk.TestFailed case
                    break;

                case ITestFinished testFinished:                                        
                    // Code to handle Xunit.Sdk.TestFinished case
                   break;

                case ITestCaseFinished testCaseFinished:
                    // Code to handle Xunit.Sdk.TestCaseFinished case
                    break;


                case ITestMethodFinished testMethodFinished:
                    // Code to handle Xunit.Sdk.TestMethodFinished case
                    break;


                default:
                    LogMessage($"UNKNOWN: {message.GetType().Name}");
                    break;
            }
        }
    }
}
