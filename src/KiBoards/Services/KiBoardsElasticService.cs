using KiBoards.Models;
using Nest;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsElasticService : IKiBoardsElasticService
    {
        private readonly IElasticClient _elasticClient;
        private readonly IMessageSink _messageSink;

        public KiBoardsElasticService(IElasticClient elasticClient, IMessageSink messageSink)
        {
            _elasticClient = elasticClient;
            _messageSink = messageSink;
        }


        public async Task IndexTestCasesStatusAsync(IEnumerable<KiBoardsTestCaseStatus> testCases)
        {
            try
            {
                _messageSink.OnMessage(new DiagnosticMessage($"Indexing {testCases.Count()} test cases"));
                var result = await _elasticClient.IndexManyAsync(testCases);

                if (!result.IsValid)
                {
                    _messageSink.OnMessage(new DiagnosticMessage(result.DebugInformation));
                }
            }
            catch (Exception ex)
            {
                _messageSink.OnMessage(new DiagnosticMessage(ex.Message));
            }
        }

        public async Task IndexTestCaseRunAsync(KiBoardsTestCaseRun testCase)
        {
            try
            {
                var result = await _elasticClient.IndexDocumentAsync(testCase);

                if (!result.IsValid)
                {
                    _messageSink.OnMessage(new DiagnosticMessage(result.DebugInformation));
                }
            }
            catch (Exception ex)
            {
                _messageSink.OnMessage(new DiagnosticMessage(ex.Message));
            }
        }

        public async Task IndexTestCaseStatusAsync(KiBoardsTestCaseStatus testCase)
        {
            try
            {
                var result = await _elasticClient.IndexDocumentAsync(testCase);

                if (!result.IsValid)
                {
                    _messageSink.OnMessage(new DiagnosticMessage(result.DebugInformation));
                }
            }
            catch (Exception ex)
            {
                _messageSink.OnMessage(new DiagnosticMessage(ex.Message));
            }
        }
    }
}
