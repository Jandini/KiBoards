using Xunit.Sdk;

namespace KiBoards
{
    public class TestRun
    {
        public Guid Id {  get; internal set; }
        public DateTime DateTime { get; internal set; }
        public string MachineName { get; internal set; }
        public string UserName { get; internal set; }        
        public object Context { get; internal set; }
        internal RunSummary RunSummary { get; set; }

        public TestRun()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;        
        }
    }
}
