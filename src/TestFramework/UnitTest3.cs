using Xunit.Abstractions;

namespace TestFramework
{

    public class UnitTest3 
    {

        public UnitTest3(ITestOutputHelper outputHelper)
        {
            // This is how to get messageBus


            outputHelper.WriteLine("HELLO WORLD MESSAGE BUS");
        }

        [Fact(Timeout = 1000)]
        public void Test5()
        {
            Thread.Sleep(5000);


        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void Test6(int a, int b)
        {
            Assert.Equal(a, b);
        }


        [Fact]
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