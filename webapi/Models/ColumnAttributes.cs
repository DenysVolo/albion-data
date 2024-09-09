[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ColumnNameAttribute : Attribute
{
    public string Name { get; }

    public string Comparator { get; }
    
    public ColumnNameAttribute(string name, string comparator)
    {
        Name = name;
        Comparator = comparator;
    }
}