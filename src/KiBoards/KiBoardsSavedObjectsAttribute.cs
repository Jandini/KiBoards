[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class KiBoardsSavedObjectsAttribute : Attribute
{
    public bool Overwrite { get; set; }
    public string SearchPattern { get; set; }

    public KiBoardsSavedObjectsAttribute(string searchPattern = "*.ndjson") 
    {
        SearchPattern = searchPattern;
    }
}