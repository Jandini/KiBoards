using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]


namespace KiBoards.Tests
{
    
    public class UnitTest1 
    {
        readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper) 
        {
            _testOutputHelper = testOutputHelper;
            
        }

        [Fact]
        public void Test1()
        {                       
            
            Thread.Sleep(1000);
        }

        //[Fact]
        //public void Test2()
        //{
        //    Assert.Equal(1, 2);
        //}


        //[Fact]
        //public void Test3()
        //{
        //    throw new NotImplementedException();
        //}

        [Fact(Skip = "Not required.")]
        public void Test4()
        {

        }

    }
}