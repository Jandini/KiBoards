namespace KiBoards.Models
{
    class KiBoardsTestCaseStatus
    {
        public string UniqueId { get; set; }
        public string DisplayName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string SkipReason { get; set; }
        public KiBoardsTestCaseMethod Method { get; set; }
        public KiBoardsTestCaseClass Class { get; set; }
        public object Context { get; set; }
    }
}
