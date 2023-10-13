using Xunit.Abstractions;

namespace KiBoards.Models
{
    class KiBoardsTestCase
    {
        public ISourceInformation SourceInformation { get; set; }
        public Dictionary<string, List<string>> Traits { get; set; }
        public string DisplayName { get; internal set; }
        public string UniqueId { get; internal set; }
        public string MethodName { get; internal set; }
        public KiBoardsTestCaseClass Class { get; internal set; }
        public KiBoardsTestCaseCollection Collection { get; internal set; }
        public KiBoardsTestCaseMethod Method { get; set; }
    }
}