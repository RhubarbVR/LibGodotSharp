using GDExtension;
using LibGodotSharp;

namespace GodotGame;

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
        var e = new Label();
        e.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        var script = new GDScript();
        script.SourceCode =
@"
extends Label

func _ready():
	print(get_tree().root.get_class())

";
        script.Reload();
        newNode.AddChild(e);
        scene.Root.AddChild(newNode);
        e.SetScript(script);
    }
}