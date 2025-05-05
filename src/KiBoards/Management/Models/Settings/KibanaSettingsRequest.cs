namespace KiBoards.Management.Models.Settings;

internal class KibanaSettingsRequest<T> where T : class
{
    public T Changes { get; set; }
}
