namespace GDExtension;

public unsafe partial class PackedStringArray {

	public string this[int index] {
		get => this[(long)index];
	}

	public string this[long index] {
		get {
			var res = GDExtensionMain.extensionInterface.packed_string_array_operator_index(_internal_pointer, index);
			return StringMarshall.ToManaged(res);
		}
	}
}
