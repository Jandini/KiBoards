using Xunit.Abstractions;

namespace KiBoards.Services
{
    internal interface IKiBoardsTestRunnerService 
    {
        Task IndexTestRunAsync(TestRun testRun);
        Task IndexTestCaseRunAsync(ITestResultMessage testResult);
    }
}