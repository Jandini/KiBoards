
namespace TestStartup
{
    public class StartupExecution_Tests
    {

        [Fact]
        public void CreateInstance()
        {
            Assert.True(Startup.InstanceCreated);
        }
    }
}