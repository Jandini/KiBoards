using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Models
{
    internal static class KiBoardsModelsExtensions
    {

        internal static KiBoardsTestCaseStatus ToKiBoardsTestCase(this IXunitTestCase testCase, ITestMethod testMethod, KiBoardsTestCaseStatusName status, KiBoardsTestCaseState state, object context = null) => new KiBoardsTestCaseStatus()
        {
            UniqueId = testCase.UniqueID,
            DisplayName = testCase.DisplayName,
            SkipReason = testCase.SkipReason,
            UpdatedOn = DateTime.Now.ToUniversalTime(),
            Status = status.ToString(),
            State = state.ToString(),
            Method = new KiBoardsTestCaseMethod()
            {
                Name = testMethod?.TestClass.Class.Name
            },
            Class = new KiBoardsTestCaseClass()
            {
                Assembly = new KiBoardsTestCaseAssembly()
                {
                    Name = testMethod?.TestClass.Class.Assembly.Name,
                    AssemblyPath = testMethod?.TestClass.Class.Assembly.AssemblyPath,
                },
                Name = testMethod?.TestClass.Class.Name,                
            },
            Context = context
        };

    
        internal static IEnumerable<KiBoardsTestCaseStatus> ToKiBoardsTestCases(this IEnumerable<IXunitTestCase> testCases, KiBoardsTestCaseStatusName status, KiBoardsTestCaseState state, object context = null) =>
            testCases.Select(x => x.ToKiBoardsTestCase(null, status, state, context));

        internal static KiBoardsTestCaseStatusName ToKiBoardsTestCaseStatus(this RunSummary summary)
            => summary.Failed > 0 ? KiBoardsTestCaseStatusName.Failure : summary.Skipped > 0 ? KiBoardsTestCaseStatusName.Skipped : KiBoardsTestCaseStatusName.Success;
    }
}
