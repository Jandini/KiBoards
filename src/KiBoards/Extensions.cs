using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    internal static class Extensions
    {     
        public static void WriteMessage(this IMessageSink messageSink, string message, string method = "Startup") => messageSink.OnMessage(new DiagnosticMessage($"{method}: {message}"));
        public static void WriteException(this IMessageSink messageSink, string message, Exception exception, string method = "Startup") => messageSink.OnMessage(new DiagnosticMessage($"{message} {exception.Message}{(exception.InnerException != null ? "\n" + exception.InnerException.Message : "")}"));
    }
}
