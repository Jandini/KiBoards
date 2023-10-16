using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    internal static class Extensions
    {     
        public static void WriteMessage(this IMessageSink messageSink, string message) => messageSink.OnMessage(new DiagnosticMessage(message));
        public static void WriteException(this IMessageSink messageSink, Exception exception) => messageSink.OnMessage(new DiagnosticMessage("{0}\n{1}", exception.Message, exception.StackTrace));
    }
}
