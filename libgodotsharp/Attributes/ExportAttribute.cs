namespace GDExtension;

[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false)]
public class ExportAttribute : System.Attribute
{

	public ExportAttribute() { }
}
