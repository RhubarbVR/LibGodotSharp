namespace GDExtension;

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
public class RegisterAttribute : System.Attribute
{
	public string name;
	//string? icon;
	public bool editorOnly;

	public RegisterAttribute() { }
}
