using KiBoards.Models;

namespace KiBoards.Services
{
    internal interface IKiBoardsElasticService
    {
        Task IndexTestCasesStatusAsync(IEnumerable<KiBoardsTestCaseStatusDto> testCases);
        Task IndexTestCaseStatusAsync(KiBoardsTestCaseStatusDto testCase);
        Task IndexTestCaseRunAsync(KiBoardsTestCaseRun testCase);

    }
}