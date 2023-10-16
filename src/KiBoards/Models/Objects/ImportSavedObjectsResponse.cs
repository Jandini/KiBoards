namespace KiBoards.Models.Objects
{
    class ImportObjectsResponse
    {
        public int SuccessCount { get; set; }
        public bool Success { get; set; }
        public List<ImportObjectsErrors> Errors { get; set; }
    }
}
