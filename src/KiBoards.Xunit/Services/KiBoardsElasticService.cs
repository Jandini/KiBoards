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
       

        public async Task IndexDocumentAsync<T>(T document) where T: class
        {
            try
            {
                var result = await _elasticClient.IndexDocumentAsync(document);

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
