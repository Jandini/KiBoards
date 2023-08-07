using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Models
{
    internal static class KiBoardsModelsExtensions
    {

        internal static KiBoardsTestCaseStatusDto ToKiBoardsTestCase(this IXunitTestCase testCase, ITestMethod testMethod, KiBoardsTestCaseStatus status, KiBoardsTestCaseState state) => new KiBoardsTestCaseStatusDto()
        {
            UniqueId = testCase.UniqueID,
            DisplayName = testCase.DisplayName,
            SkipReason = testCase.SkipReason,
            UpdatedOn = DateTime.Now.ToUniversalTime(),
            Status = status.ToString(),
            State = state.ToString(),
            TestMethod = new KiBoardsTestMethodDto()
            {
                TestClass = new KiBoardsTestClassDto()
                {
                    Name = testMethod?.TestClass.Class.Name
                },                
                Method = new KiBoardsTestMethodInfoDto()
                {
                    Name = testMethod?.Method.Name
                }
            }            
        };

    
        internal static IEnumerable<KiBoardsTestCaseStatusDto> ToKiBoardsTestCases(this IEnumerable<IXunitTestCase> testCases, KiBoardsTestCaseStatus status, KiBoardsTestCaseState state) =>
            testCases.Select(x => x.ToKiBoardsTestCase(null, status, state));

        internal static KiBoardsTestCaseStatus ToKiBoardsTestCaseStatus(this RunSummary summary)
            => summary.Failed > 0 ? KiBoardsTestCaseStatus.Failure : summary.Skipped > 0 ? KiBoardsTestCaseStatus.Skipped : KiBoardsTestCaseStatus.Success;
    }
}
