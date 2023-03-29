using KiBoards.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace KiBoards.Tests
{
    public class UnitTest1 : IClassFixture<TestHarness>
    {

        private readonly ITestOutputHelper _output;
        private readonly TestHarness _harness;        

        public UnitTest1(ITestOutputHelper output, TestHarness builder)
        {
            _harness = builder;
            _output = output;

            _harness.SetOutputHelper(_output);
        }

        [Fact]
        public void Test1()
        {            
            _harness.AddServices();

            _harness.Run<UnitTest1>((services, logger) =>
            {
                logger.LogInformation("Test1");                
                throw new Exception("Something went wrong.");
            });
        }


        [Fact]
        public void Test2()
        {
            _harness.Run<UnitTest1>((services, logger) =>
            {
                logger.LogInformation("Test2");                
            });
        }
    }
}