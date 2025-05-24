namespace KiBoards.Management.Models.Status;

public class KibanaVersion
{
    public string Number { get; set; }
    public string BuildHash { get; set; }
    public int BuildNumber { get; set; }
    public bool BuildSnapshot { get; set; }
}
