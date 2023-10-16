[AttributeUsage(AttributeTargets.Assembly)]
public class KiboardsTestStartupAttribute : Attribute
{
    public string ClassName { get; set; }

    public KiboardsTestStartupAttribute(string className) 
    { 
        ClassName = className;
    }
}
