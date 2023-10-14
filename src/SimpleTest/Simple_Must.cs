[assembly: TestFramework("KiBoards.TestFramework", "KiBoards.Xunit")]

namespace SimpleTest
{   
    public class Simple_Must
    {
        [Theory]
        [InlineData(1, 2)]
        public void Not_DivideByZero(int a, int b)
        {
            var c = 1 / b;

            Assert.Equal(0, a);
        }
    }
}