using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;
using KiBoards.Services;

namespace KiBoards.Framework
{
    internal class TestMethodRunner : XunitTestMethodRunner
    {
        private readonly IKiBoardsTestRunnerService _testRunner;


        public TestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments, IKiBoardsTestRunnerService testRunner)
             //            : base(testMethod, @class, method, testCases, diagnosticMessageSink, new TestMessageBus(messageBus, diagnosticMessageSink), aggregator, cancellationTokenSource, constructorArguments)
             : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)

        {
            _testRunner = testRunner;
        }

        protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
        {
            try
            {
                await _testRunner.StartTestCaseAsync(testCase, TestMethod);
                var result = await base.RunTestCaseAsync(testCase);                
                await _testRunner.FinishTestCaseAsync(testCase, TestMethod, Aggregator, result);

                
                return result;
            }
            catch (Exception ex)
            {
                await _testRunner.ErrorTestCaseAsync(testCase, TestMethod, ex);
                throw;
            }
        }
    }
}
