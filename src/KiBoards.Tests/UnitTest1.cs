using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("KiBoards.TestFramework", "KiBoards")]

namespace KiBoards.Tests
{

    public class UnitTest1 : IClassFixture<TestContextFixture>
    {
        readonly TestContextFixture _testContextFixture;
        readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(TestContextFixture testContextFixture, ITestOutputHelper testOutputHelper) 
        {
            _testContextFixture = testContextFixture;
            _testOutputHelper = testOutputHelper;

            _testContextFixture.SetContext(new { Version = "Context via Fixture", Hello = "World", Input = 1 });
        }

        [Fact]
        public void Test1()
        {           
            var testCase = _testOutputHelper.GetTestCase();

            _testContextFixture.SetContext(new { Version = "12345", TestCase = testCase, TestFramework.RunIdentifier }) ;
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