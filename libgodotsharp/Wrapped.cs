namespace GDExtension;

public unsafe class Wrapped {

	public void* _internal_pointer;

	protected Wrapped(StringName type) {
		_internal_pointer = GDExtensionMain.extensionInterface.classdb_construct_object(type._internal_pointer);
	}
	protected Wrapped(void* data) => _internal_pointer = data;
	public static unsafe void __Notification(void* instance, int what) { }
	public static void Register() { }
}
