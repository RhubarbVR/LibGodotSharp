namespace GDExtension;

public unsafe partial class NodePath {

	public static implicit operator NodePath(string text) => new NodePath(text);

	public static implicit operator string(NodePath from) {
		var constructor = GDExtensionMain.extensionInterface.variant_get_ptr_constructor((Native.GDExtensionVariantType)Variant.Type.String, 3);
		var args = stackalloc void*[1];
		args[0] = from._internal_pointer;
		void* res;
		constructor(&res, args);
		return StringMarshall.ToManaged(res);
	}
}
