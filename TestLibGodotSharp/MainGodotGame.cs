using GDExtension;
using LibGodotSharp;

namespace TestLibGodotSharp
{
    public class MainGodotGame
    {
        public static HelloFromLibGodot AddCube(Vector3 pos)
        {
            var meshRender = new HelloFromLibGodot();
            meshRender.Position = pos;
            return meshRender;
        }

        public static void LoadScene(SceneTree scene)
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
    }
}