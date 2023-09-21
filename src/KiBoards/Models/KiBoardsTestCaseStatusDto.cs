namespace KiBoards.Models
{
    class KiBoardsTestCaseStatusDto
    {
        public string UniqueId { get; set; }
        public string DisplayName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string SkipReason { get; set; }
        public KiBoardsTestMethodDto TestMethod { get; set; }
        public object Context { get; set; }
    }
}
