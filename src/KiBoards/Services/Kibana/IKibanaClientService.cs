namespace KiBoards.Services
{
    public interface IKibanaClientService
    {
        Task SetDarkModeAsync(CancellationToken cancellationToken);
        Task WaitForKibanaAsync(CancellationToken cancellationToken);
    }
}
