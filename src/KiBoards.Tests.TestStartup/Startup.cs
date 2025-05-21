using System.Reflection;
using Xunit.Abstractions;

namespace TestStartup
{
    public class Startup 
    {

        public static bool InstanceCreated = false;
        public Startup(IMessageSink messageSink)
        {
            InstanceCreated = true;
            Environment.SetEnvironmentVariable("KIB_VAR_VERSION", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);            
        }     
    }
}
