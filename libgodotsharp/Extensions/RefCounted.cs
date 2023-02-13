namespace GDExtension;

public partial class RefCounted {

	~RefCounted() {
		Unreference();
	}
}
