namespace KiBoards.Models
{
    class KiBoardsTestCaseRun
    {
        public string Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public KiBoardsTestRun TestRun { get; set; }
        public KiBoardsTestCase TestCase { get; set; }
        public KiBoardsTestCaseRunSkipped Skipped { get; set; }
        public KiBoardsTestCaseRunFailed Failed { get; set; }
        public decimal ExecutionTime { get; set; }
        public string Status { get; set; }
        public string Output { get; set; }
        
    }
}
