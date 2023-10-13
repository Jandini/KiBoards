using Xunit.Sdk;

namespace KiBoards
{
    public class TestRun
    {
        public string Id { get; internal set; }
        public DateTime StartTime { get; internal set; }
        public string MachineName { get; internal set; }
        public string UserName { get; internal set; }
        public object Context { get; internal set; }
        public RunSummary Summary { get; set; }

        public TestRun()
        {
            Id = Environment.GetEnvironmentVariable("KIBOARDS_TEST_RUN_ID") ?? Guid.NewGuid().ToString();
            StartTime = DateTime.UtcNow;
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;
        }
    }
}
