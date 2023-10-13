using Xunit.Sdk;

namespace KiBoards.Models
{
    internal class KiBoardsTestCaseRunDto
    {
        public Guid RunIdentifier { get; set; }
        public string UniqueId { get; set; }
        public string DisplayName { get; set; }
        public decimal ExecutionTime { get; set; }
        public string Output { get; set; }
        public string Status {  get; set; }

        //public TestPassed Passed { get; set;}

        //public TestSkipped Skipped { get; set; }
        //public TestFailed Failed { get; set; }

    }
}
