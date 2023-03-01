using GDExtension;

namespace GodotGame;

[Register]
public partial class ZNode : Node3D
{
    [Notify(NotificationReady)]
    public void Ready()
    {
        SetProcess(true);
        var meshRender = new MeshInstance3D
        {
            Mesh = new BoxMesh()
        };
        AddChild(meshRender);
    }

    [Notify(NotificationProcess, arguments = "GetProcessDeltaTime()")]
    public void Process(double delta)
    {
        RotateY(delta);
    }
}
