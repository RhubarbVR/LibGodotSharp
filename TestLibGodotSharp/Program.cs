using GDExtension;
using LibGodotSharp;

namespace TestLibGodotSharp
{
    internal class Program
    {
        static MeshInstance3D AddCube(Vector3 pos)
        {
            var meshRender = new MeshInstance3D();
            meshRender.Position = pos;
            meshRender.Mesh = new BoxMesh();
            return meshRender;
        }

        static void LoadScene(SceneTree scene)
        {
            var newNode = new Node3D();
            var cam = new Camera3D();
            cam.Current = true;
            cam.Position = new Vector3(0, 0, 2);
            newNode.AddChild(cam);
            newNode.AddChild(AddCube(new Vector3(1, 1, 1)));
            newNode.AddChild(AddCube(new Vector3(-1, -1, -1)));
            newNode.AddChild(AddCube(new Vector3(0, 1, 1)));
            scene.Root.AddChild(newNode);
            scene.SetCurrentScene(newNode);
        }

        static unsafe int Main(string[] args)
        {
            return LibGodotManager.RunGodot(args, ExtensionEntry.EntryPoint, LoadScene, true);
        }
    }
}