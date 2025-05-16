using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    internal static class Extensions
    {     
        public static void WriteMessage(this IMessageSink messageSink, string message, [CallerMemberName] string method = "") => messageSink.OnMessage(new DiagnosticMessage($"{method}: {message}"));
        public static void WriteException(this IMessageSink messageSink, string message, Exception exception, [CallerMemberName] string method = "") => messageSink.OnMessage(new DiagnosticMessage($"{message} {exception.Message}{(exception.InnerException != null ? "\n" + exception.InnerException.Message : "")}"));
    }
}
