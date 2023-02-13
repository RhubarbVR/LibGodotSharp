namespace GDExtension;

public unsafe partial class Array {

	public Variant this[int index] {
		get => this[(long)index];
	}

	public Variant this[long index] {
		get {
			var res = GDExtensionMain.extensionInterface.array_operator_index(_internal_pointer, index);
			return new Variant(res);
		}
	}
}
