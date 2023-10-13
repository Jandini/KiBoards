using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal interface IKiBoardsTestRunnerService 
    {        
        Task BeginTestCasesRunAsync(IEnumerable<IXunitTestCase> testCases);
        Task StartTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod);
        Task FinishTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod, ExceptionAggregator exceptionAggregator, RunSummary result);
        Task ErrorTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod, Exception ex);
        Task EndTestCasesRunAsync(RunSummary results);
        Task ErrorTestCasesRunAsync(IEnumerable<IXunitTestCase> testCases, Exception ex);
        Task IndexTestCaseRunAsync(ITestResultMessage testResult);
    }
}