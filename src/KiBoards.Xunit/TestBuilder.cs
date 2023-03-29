using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiBoards.Xunit
{
    public class TestBuilder : IDisposable
    {
        private ServiceProvider? _serviceProvider;
        public IServiceCollection Services { get; private set; }
        public IConfigurationBuilder Configuration { get; private set; }

        public TestBuilder()
        {
            Services = new ServiceCollection();
            Configuration = new ConfigurationBuilder();
        }

        public IServiceProvider BuildServiceProvider() 
        {            
            return _serviceProvider = Services.BuildServiceProvider();
        }

        public void Dispose()
        {            
            _serviceProvider?.Dispose();
        }
    }
}
