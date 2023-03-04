using GDExtension;
using LibGodotSharp;

namespace GodotGame;

public class MainGodotGame
{
    public static bool RunInVR = false;

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
        var data = GDExtension.ProjectSettings.GetSetting("display/window/vsync/vsync_mode");
        Console.WriteLine(data.NativeType);
        Console.WriteLine(Variant.VariantToObject(data));
        ProjectSettings.SetSetting("application/config/name", "TestConsoleApp");
        if (RunInVR)
        {
            ProjectSettings.SetSetting("xr/openxr/enabled", true);
            ProjectSettings.SetSetting("xr/shaders/enabled", true);
            ProjectSettings.SetSetting("display/window/vsync/vsync_mode", 0);
        }
    }

    static HelloFromLibGodot[] allCubes = System.Array.Empty<HelloFromLibGodot>();
    
    public static void Message()
    {
        _counter++;

        Console.WriteLine($"ButtonPress {_counter}");
        if ((_counter%2) == 1)
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
        if ((_counter % 2) == 0)
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

    public unsafe static void LoadScene(SceneTree scene)
    {
        _sceneTree = scene;

        foreach (var item in InputMap.GetActions())
        {
            Console.WriteLine("InputAction "+item);
        }

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

        var testNode = new Node3D();
        newNode.AddChild(testNode);
        foreach (var item in newNode.GetChildren())
        {
            Console.WriteLine($"{item}");
        }

        newNode.AddChild(new ANode()); // Tests funny bug
        newNode.AddChild(new ZNode());

        var array = (PackedInt32Array)new int[] { 3, 3, 3, 3, 435, 345, 3453, 53, 2, 34, 4, 23, 4, 2, 43, 4543534, 435342, 1, 2342345, 5345, 345, 345, 43, 543, 645, 67, 5676, 8, 64534, 5, 345, 3456, 45, 67, 76, 756867, 1, 34, 534, 5, 345, 34, 534, 634, 5, 3456, 3 };
        var stringArray = (PackedStringArray)new string[] { "This", "IS", "A", "Packed", "Sting", "ARRAY", "I", "AM", "Testing", "It", "By", "ADDing", "STUFF", "TO", "it", "It", "Might", "be", "COOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOL", "IFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF STUFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF JUSTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT WORKSDSDEDF!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
        array.Size();
        stringArray.Size();
        for (int i = 0; i < array.Size(); i++)
        {
            Console.WriteLine(array[i]);
        }
        for (int i = 0; i < stringArray.Size(); i++)
        {
            Console.WriteLine(stringArray[i]);
        }
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
        scene.Root.UseXr = RunInVR;
        scene.Root.AddChild(newNode);
        e.SetScript(script);
    }
}