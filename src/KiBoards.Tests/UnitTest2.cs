using Xunit.Abstractions;

namespace KiBoards.Tests
{

    [Trait("TestGroup", "UnitTest2")]    
    public class UnitTest2 : IClassFixture<TestContextFixture>
    {

        public UnitTest2(ITestOutputHelper outputHelper)
        {            
            outputHelper.WriteLine("HELLO WORLD MESSAGE BUS");
        }

        [Fact(Skip = "Excluded", Timeout = 1000)]
        public void Test5()
        {
            Thread.Sleep(5000);

           
        }

        [Trait("TestType", "Test6")]
        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void Test6(int a, int b)
        {
            Assert.Equal(a, b);
        }


        [Fact]
        [Trait("TestType", "Test7")]
        public void Test7()
        {
            throw new NotImplementedException();
        }

        [Fact(Skip = "Not required.")]
        public void Test8()
        {

        }

    }
}