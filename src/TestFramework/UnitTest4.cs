using Xunit.Abstractions;

namespace TestFramework
{
    public class UnitTest4
    {
        readonly ITestOutputHelper _testOutputHelper;

        public UnitTest4(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

        }

        [Fact]
        public void Test1()
        {

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
