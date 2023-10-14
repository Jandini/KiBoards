namespace KiBoards.Models
{
    class KiBoardsTestCaseRun
    {
        public TestRun TestRun { get; set; }
        public KiBoardsTestCase TestCase { get; set; }
        public KiBoardsTestCaseRunSkipped Skipped { get; set; }
        public KiBoardsTestCaseRunFailed Failed { get; set; }
        public decimal ExecutionTime { get; set; }
        public string Status { get; set; }
        public string Output { get; set; }
    }
}
