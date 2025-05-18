using KiBoards.Management.Models.Status;

namespace KiBoards.Management;

public class KibanaStatusResponse
{
    public string Name { get; set; }
    public string Uuid { get; set; }
    public KibanaVersion Version { get; set; }
    public KibanaStatus Status { get; set; }
}
