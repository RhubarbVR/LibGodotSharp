namespace GDExtension;

public unsafe partial class Node : Object
{

    public T GetNode<T>(NodePath path) where T : Node => (T)GetNode(path);

    public T AddChild<T>(bool force_readable_name = (bool)false, Node.InternalMode @internal = (Node.InternalMode)0) where T : Node ,new()
    {
        var node = new T();
        AddChild(node, force_readable_name, @internal);
        return node;
    }



}
