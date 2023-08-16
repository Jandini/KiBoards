using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Framework
{
    internal class TestMessageSink : IMessageSink
    {
        private IMessageSink _messageSink;

        internal TestMessageSink(IMessageSink messageSink) 
        {
            _messageSink = messageSink;
        }

        private void LogMessage(string message)
        {
            _messageSink.OnMessage(new DiagnosticMessage($"MessageSink: {message}"));
        }


        public bool OnMessage(IMessageSinkMessage message)
        {
            HandleMessageSinkMessage(message);
            return _messageSink.OnMessage(message);
        }

        private void HandleMessageSinkMessage(IMessageSinkMessage message)
        {
            switch (message)
            {

                case ITestContextMessage testContext:
                    break;

                case ITestAssemblyStarting testAssemblyStarting:
                    LogMessage(message.GetType().Name);
                    break;

                case ITestCollectionStarting testCollectionStarting:
                    LogMessage(message.GetType().Name);
                    break;




                case ITestMethodStarting testMethodStarting:
                    LogMessage($"TestMethodStarting: TestCollection.DisplayName: {testMethodStarting.TestCollection.DisplayName}");
                    LogMessage($"TestMethodStarting: TestCollection.UniqueID: {testMethodStarting.TestCollection.UniqueID}");
                    LogMessage($"TestMethodStarting: TestCollection.TestAssembly.Assembly.AssemblyPath: {testMethodStarting.TestCollection.TestAssembly.Assembly.AssemblyPath}");
                    LogMessage($"TestMethodStarting: TestCases.Count: {testMethodStarting.TestCases.Count()}");

                    LogMessage($"TestMethodStarting: TestClass.Class.Name: {testMethodStarting.TestClass.Class.Name}");
                    break;

                case ITestCaseStarting testCaseStarting:
                    LogMessage("TestCaseStarting");
                    break;

                case ITestStarting testStarting:
                    LogMessage($"TestStarting: Test.DisplayName: {testStarting.Test.DisplayName}");
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
