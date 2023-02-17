using GDExtension;

namespace TestLibGodotSharp
{
    [Register]
    public partial class HelloFromLibGodot : Node3D
    {
        [Notify(NotificationReady)]
        public void Ready()
        {
            SetProcess(true);
            var meshRender = new MeshInstance3D();
            meshRender.Mesh = new BoxMesh();
            AddChild(meshRender);
        }

        [Notify(NotificationProcess, arguments = "GetProcessDeltaTime()")]
        public void Process(double delta)
        {
            RotateY(delta);
        }
    }
}