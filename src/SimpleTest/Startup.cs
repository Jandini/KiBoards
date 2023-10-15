using System.Reflection;
using Xunit.Abstractions;

namespace SimpleTest
{
    public class Startup 
    {
        public Startup(IMessageSink messageSink)
        {
            Environment.SetEnvironmentVariable("KIB_VAR_VERSION", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);            
        }     
    }
}
