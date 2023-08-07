using Xunit;
using Xunit.Sdk;

[assembly: TestFramework("KiBoards.TestFramework", "KiBoards")]

namespace KiBoards.Tests
{

    public class UnitTest1 : IClassFixture<TestContextFixture>
    {
        TestContextFixture _kiboards;

        public UnitTest1(TestContextFixture testContextFixture) 
        {
            _kiboards = testContextFixture;

            _kiboards.SetContext(new { Version = "12345" });
        }

        [Fact]
        public void Test1()
        {
            _kiboards.SetContext(new { Version = "12345" });

            Thread.Sleep(1000);
        }

        [Fact]
        public void Test2()
        {
            Assert.Equal(1, 2);
        }


        [Fact]
        public void Test3()
        {
            throw new NotImplementedException();
        }

        [Fact(Skip = "Not required.")]
        public void Test4()
        {

        }

    }
}