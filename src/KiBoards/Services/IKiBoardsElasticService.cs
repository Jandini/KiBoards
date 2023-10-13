using KiBoards.Models;

namespace KiBoards.Services
{
    internal interface IKiBoardsElasticService
    {
        Task IndexTestCasesStatusAsync(IEnumerable<KiBoardsTestCaseStatus> testCases);
        Task IndexTestCaseStatusAsync(KiBoardsTestCaseStatus testCase);
        Task IndexTestCaseRunAsync(KiBoardsTestCaseRun testCase);

    }
}