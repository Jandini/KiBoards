namespace KiBoards
{
    public class TestStartupFixture 
    {        
        public void SetContext(object context)
        {
            TestFramework.TestRun.Context = context; 
        }
    }
}
