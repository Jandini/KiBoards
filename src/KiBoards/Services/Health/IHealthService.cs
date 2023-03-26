namespace KiBoards.Services
{
    public interface IHealthService
    {
        Task<HealthInfo> GetHealthInfoAsync();
    }
}