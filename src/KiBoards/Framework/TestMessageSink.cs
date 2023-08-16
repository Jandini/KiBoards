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
                    LogMessage($"TestAssemblyStarting: StartTime: {testAssemblyStarting.StartTime}");
                    LogMessage($"TestAssemblyStarting: TestFrameworkDisplayName: {testAssemblyStarting.TestFrameworkDisplayName}");
                    LogMessage($"TestAssemblyStarting: TestEnvironment: {testAssemblyStarting.TestEnvironment}");
                    LogMessage($"TestAssemblyStarting: TestCases.Count: {testAssemblyStarting.TestCases.Count()}");
                    break;

                case ITestCollectionStarting testCollectionStarting:
                    LogMessage($"TestCollectionStarting: TestCollection.DisplayName: {testCollectionStarting.TestCollection.DisplayName}");
                    LogMessage($"TestCollectionStarting: TestCollection.UniqueID: {testCollectionStarting.TestCollection.UniqueID}");
                    LogMessage($"TestCollectionStarting: TestCollection.TestCases.Count: {testCollectionStarting.TestCases.Count()}");
                    LogMessage($"TestCollectionStarting: TestAssembly.Assembly.Name: {testCollectionStarting.TestAssembly.Assembly.Name}");
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
                    LogMessage($"TestClassConstructionStarting: Test.DisplayName: {classConstructionStarting.Test.DisplayName}");
                    LogMessage($"TestClassConstructionStarting: Test.TestCase.DisplayName: {classConstructionStarting.Test.TestCase.DisplayName}");
                    LogMessage($"TestClassConstructionStarting: Test.TestCase.UniqueID: {classConstructionStarting.Test.TestCase.UniqueID}");
                    LogMessage($"TestClassConstructionStarting: Test.TestCase.TestMethod.Method.Name: {classConstructionStarting.Test.TestCase.TestMethod.Method.Name}");
                    break;

                case ITestClassConstructionFinished classConstructionFinished:
                    // Code to handle Xunit.Sdk.TestClassConstructionFinished case
                    break;


                case ITestSkipped testSkipped:
                    // Code to handle Xunit.Sdk.TestSkipped case
                    break;

                case ITestPassed testPassed:
                    LogMessage($"TestPassed: Output: {testPassed.Output}");
                    LogMessage($"TestPassed: ExcecutionTime: {testPassed.ExecutionTime}");
                    LogMessage($"TestPassed: Test.DisplayName: {testPassed.Test.DisplayName}");
                    LogMessage($"TestPassed: TestCase.UniqueID: {testPassed.TestCase.UniqueID}");
                    LogMessage($"TestPassed: TestCase.DisplayName: {testPassed.TestCase.DisplayName}");
                    LogMessage($"TestPassed: TestCases.Count: {testPassed.TestCases.Count()}");

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

                case ITestClassStarting testClassStarting:
                    break;

                case ITestOutput TestOutput:
                    break;

                case ITestClassFinished testClassFinished:
                    break;

                case ITestCollectionFinished testCollectionFinished:
                    break;

                case ITestAssemblyFinished testAssemblyFinished:
                    LogMessage($"TestAssemblyFinished: TestsRun: {testAssemblyFinished.TestsRun}");
                    LogMessage($"TestAssemblyFinished: TestsFailed: {testAssemblyFinished.TestsFailed}");
                    LogMessage($"TestAssemblyFinished: TestsSkipped: {testAssemblyFinished.TestsSkipped}");
                    LogMessage($"TestAssemblyFinished: ExecutionTime: {testAssemblyFinished.ExecutionTime}");
                    break;

                default:
                    LogMessage($"UNKNOWN: {message.GetType().Name}");
                    break;
            }
        }
    }
}
