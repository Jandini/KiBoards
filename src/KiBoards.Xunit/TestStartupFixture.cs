namespace KiBoards
{
    public class TestStartupFixture : IDisposable
    {
        public void SetContext(object context)
        {
            TestFramework.TestRun.Context = context;
        }

        public void Dispose()
        {
            
        }
    }
}
