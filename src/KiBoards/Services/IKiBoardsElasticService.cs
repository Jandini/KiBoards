namespace KiBoards.Services
{
    internal interface IKiBoardsElasticService
    {
        Task IndexDocumentAsync<T>(T document) where T : class;

    }
}