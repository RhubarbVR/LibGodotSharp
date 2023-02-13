namespace GDExtension;

public unsafe partial class Node : Object {

	public T GetNode<T>(NodePath path) where T : Node => (T)GetNode(path);
}
