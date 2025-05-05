namespace KiBoards.Management.Models.Objects;

class KibanaImportObjectsResponse
{
    public int SuccessCount { get; set; }
    public bool Success { get; set; }
    public List<KibanaImportObjectsErrors> Errors { get; set; }
}
