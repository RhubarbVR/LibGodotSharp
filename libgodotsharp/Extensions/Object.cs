namespace GDExtension;

public unsafe partial class Object : Wrapped {
	public static Object ConstructUnknown(void* ptr) {
		if (ptr == null) { return null!; }
		var o = new Object(ptr);
		var c = o.GetClass();
		return constructors[c](ptr);
	}

	public delegate Object Constructor(void* data);
	public static void RegisterConstructor(string name, Constructor constructor) => constructors.Add(name, constructor);

	static System.Collections.Generic.Dictionary<string, Constructor> constructors = new();
}
