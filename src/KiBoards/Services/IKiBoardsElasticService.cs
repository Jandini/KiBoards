using KiBoards.Models;

namespace KiBoards.Services
{
    internal interface IKiBoardsElasticService
    {
        Task IndexTestCasesAsync(IEnumerable<KiBoardsTestCaseStatusDto> testCases);
        Task IndexTestCaseAsync(KiBoardsTestCaseStatusDto testCase);

    }
}