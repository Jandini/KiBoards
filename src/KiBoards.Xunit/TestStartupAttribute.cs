[AttributeUsage(AttributeTargets.Assembly)]
public class TestStartupAttribute : Attribute
{
    public string ClassName { get; set; }

    public TestStartupAttribute(string className) 
    { 
        ClassName = className;
    }
}
