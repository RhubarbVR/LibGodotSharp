using GDExtension;

namespace GodotGame;

[Register]
public partial class ANode : Node3D
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

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey key)
        {
            if(key.Keycode == Key.Space)
            {
                if (key.IsPressed())
                {
                    Console.WriteLine("SPACE");
                }
            }
        }
    }
}
