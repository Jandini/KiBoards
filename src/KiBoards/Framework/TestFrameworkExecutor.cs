using System.Reflection;
using KiBoards.Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Framework
{
    internal class TestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        private readonly IKiBoardsTestRunnerService _testRunner;

        public TestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink, IKiBoardsTestRunnerService testRunner)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
            _testRunner = testRunner;
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            try
            {
                await _testRunner.BeginTestCasesRunAsync(testCases);                
                using var assemblyRunner = new XunitTestAssemblyRunner(TestAssembly, testCases, new TestMessageSink(DiagnosticMessageSink), executionMessageSink, executionOptions);
                var results = await assemblyRunner.RunAsync();
                await _testRunner.EndTestCasesRunAsync(results);
            }
            catch (Exception ex)
            {
                await _testRunner.ErrorTestCasesRunAsync(testCases, ex);
            }
        }
        
    }
}
