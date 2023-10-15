using Nest;
using System.Collections;

namespace KiBoards.Models
{
    public class KiBoardsTestRun
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string Hash { get; internal set; }
        public DateTime StartTime { get; internal set; }
        public string MachineName { get; internal set; }
        public string UserName { get; internal set; }
        public Dictionary<string, string> Variables { get; internal set; }

        [Object]
        internal KiBoardsTestRunSummary Summary { get; set; }       
    }
}
