namespace KiBoards.Tests
{
    public class TestContextFixture : TestStartupFixture
    {
        public TestContextFixture() 
        {
            SetContext(new { 
                Version = "9.0.0-alpha.123",
                Computer = Environment.MachineName,
                BranchName = Environment.GetEnvironmentVariable("BRANCH_NAME")                
            });
        }
    }
}
