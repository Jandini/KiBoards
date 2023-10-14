using Nest;

namespace KiBoards
{
    public class TestRun
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string Hash {  get; internal set; }
        public DateTime StartTime { get; internal set; }
        public string MachineName { get; internal set; }
        public string UserName { get; internal set; }
        public object Context { get; internal set; }
        

        [Object]
        internal TestRunSummary Summary { get; set; }

        public TestRun()
        {
            Id = Environment.GetEnvironmentVariable("KIBS_TEST_RUN_ID") ?? Guid.NewGuid().ToString();
            StartTime = DateTime.UtcNow;
            MachineName = Environment.MachineName;
            UserName = Environment.UserName;
        }
    }
}
