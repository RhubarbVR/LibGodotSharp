using GDExtension;

namespace Nodes; //Needs namespace for code generator

/// <summary>
/// Demo node to show how to register a node to godot
/// </summary>
[Register] // Need register attribute to know what needs to be code generator
public partial class DemoSpinningCubeNode : Node3D // Needs to be a partial class for the code generator to extended it
{
    [Notify(NotificationReady)] // Used to put actions on the Godot's notify system
    public void Ready()
    {
        SetProcess(true); // Need to tell godot that this node uses process loop manialy 

        //Adds a cube mesh for a test visual
        var meshRender = new MeshInstance3D
        {
            Mesh = new BoxMesh()
        };
        AddChild(meshRender);
    }

    [Notify(NotificationProcess, arguments = "GetProcessDeltaTime()")] // Can add arguments that just pass code through to the generator
    public void Process(double delta)
    {
        RotateY(delta); // rotate the cube to show that the process can be used
    }
}
