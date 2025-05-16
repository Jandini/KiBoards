using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    internal static class Extensions
    {     
        public static void WriteMessage(this IMessageSink messageSink, string message) => messageSink.OnMessage(new DiagnosticMessage(message));
        public static void WriteException(this IMessageSink messageSink, string message, Exception exception) => messageSink.OnMessage(new DiagnosticMessage("{0}\n{1}\n{3}", message, exception.Message, (exception.InnerException != null ? $"{exception.InnerException.Message}" : "")));
    }
}
