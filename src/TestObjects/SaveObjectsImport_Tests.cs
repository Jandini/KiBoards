[assembly: KiBoardsSavedObjects()]
[assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]

namespace TestObjects
{   
    public class SaveObjectsImport_Tests
    {
        [Theory]
        [InlineData(0, 2)]
        public void Not_DivideByZero(int a, int b)
        {
            var c = 1 / b;

            Assert.Equal(0, a);
        }

        [Fact]
        public void Pass()
        {
            Assert.Equal(0, 0);

            Thread.Sleep(6000);
        }
    }
}