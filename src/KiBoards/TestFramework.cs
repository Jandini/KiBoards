using System.Reflection;
using KiBoards.Framework;
using KiBoards.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    public class TestFramework : XunitTestFramework, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public TestFramework(IMessageSink messageSink)
            : base(messageSink)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton(messageSink)
                .AddElasticServices()                
                .AddSingleton<IKiBoardsTestRunnerService, KiBoardsTestRunnerService>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink, _serviceProvider.GetRequiredService<IKiBoardsTestRunnerService>());
        }

        public new async void Dispose()
        {
            await Task.Delay(1);
            _serviceProvider.Dispose();
            base.Dispose();
        }
    }
}

