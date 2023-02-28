using GDExtension;
using LibGodotSharp;

namespace GodotGame;

public class MainGodotGame
{
    public static HelloFromLibGodot AddCube(Vector3 pos)
    {
        var meshRender = new HelloFromLibGodot
        {
            Position = pos
        };
        return meshRender;
    }

    public static void LoadProjectSettings(ProjectSettings projectSettings)
    {
        ProjectSettings.SetSetting("application/config/name", "TestConsoleApp");
        ProjectSettings.SetSetting("xr/openxr/enabled", true);
        ProjectSettings.SetSetting("xr/shaders/enabled", true);
        ProjectSettings.SetSetting("display/window/vsync/vsync_mode", 0);
    }

    static HelloFromLibGodot[] allCubes = System.Array.Empty<HelloFromLibGodot>();
    public static void Message()
    {
        _counter++;
        Console.WriteLine($"ButtonPress {_counter}");
        if (_counter == 4)
        {
            allCubes = new HelloFromLibGodot[100];
            for (int i = 0; i < allCubes.Length; i++)
            {
                allCubes[i] = AddCube(new Vector3(i % 10, (i / 10) % 10, -5));
                _sceneTree.Root.AddChild(allCubes[i]);
            }
        }
        if (_counter == 6)
        {
            var testButton = new Button
            {
                Text = "Other button",
                ToggleMode = true,
            };
            testButton.SetPosition(new Vector2(100, 100));
            testButton.Pressed += () =>
            {
                Console.WriteLine($"LamdaPress");
            };
            _sceneTree.Root.AddChild(testButton);
        }
        if (_counter == 8)
        {
            for (int i = 0; i < allCubes.Length; i++)
            {
                allCubes[i].QueueFree();
            }
            allCubes = System.Array.Empty<HelloFromLibGodot>();
        }
        GC.Collect();
    }

    static int _counter = 0;

    public static void OtherMessage(bool trains)
    {
        Console.WriteLine($"OtherMessage {trains}");
    }

    static SceneTree _sceneTree;

    public static void LoadScene(SceneTree scene)
    {
        _sceneTree = scene;
        var newNode = new Node3D();
        var eorgin = new XROrigin3D();
        var cam = new XRCamera3D
        {
            Current = true,
            Position = new Vector3(0, 0, 2)
        };
        eorgin.AddChild(cam);
        newNode.AddChild(eorgin);
        newNode.AddChild(AddCube(new Vector3(1, 1, 1)));
        newNode.AddChild(AddCube(new Vector3(-1, -1, -1)));
        newNode.AddChild(AddCube(new Vector3(0, 1, 1)));

        var testButton = new Button
        {
            Text = "testButton",
            ToggleMode = true
        };
        testButton.Pressed += Message;
        testButton.Toggled += OtherMessage;

        newNode.AddChild(testButton);

        var e = new Label();
        e.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        var script = new GDScript
        {
            SourceCode =
@"
extends Label

func _ready():
	print(get_tree().root.get_class())

"
        };
        script.Reload();
        newNode.AddChild(e);
        scene.Root.UseXr = true;
        scene.Root.AddChild(newNode);
        e.SetScript(script);
    }
}