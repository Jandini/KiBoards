using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace KiBoards.Xunit
{
    public class UnitTest1 : IClassFixture<KiBoardsTestFixture>
    {

        private readonly ITestOutputHelper _output;
        private readonly KiBoardsTestFixture _test;        

        public UnitTest1(ITestOutputHelper output, KiBoardsTestFixture builder)
        {
            _test = builder;
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            _test.SetOutputHelper(_output);
            _test.AddServices();            
                
            _test.Run<UnitTest1>((services, logger) =>
            {
                logger.LogInformation("Test1");
                throw new Exception("Something went wrong.");
            });
        }


        [Fact]
        public void Test2()
        {
            _test.Run<UnitTest1>((services, logger) =>
            {
                logger.LogInformation("Test2");                
            });
        }
    }
}