namespace KiBoards.Xunit.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

    [FriendlyName(FriendlyName)]
    [ExtensionUri(ExtensionUri)]
    public class XunitKiBoardsLogger : ITestLoggerWithParameters
    {

        public const string ExtensionUri = "logger://Microsoft/TestPlatform/XunitKiBoardsLogger/v1";

        public const string FriendlyName = "kiboards";


        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
