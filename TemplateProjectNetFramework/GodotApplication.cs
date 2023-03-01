using GDExtension;

using Nodes;

/// <summary>
/// Main class that platform used to load godot scene after godot is initialized
/// - can only be renamed or put in a namespace if you also do it on each platform project
/// </summary>
public static class GodotApplication
{
    /// <summary>
    /// Creates new demo cube at location
    /// </summary>
    /// <param name="pos">location to put spinning cube</param>
    /// <returns></returns>
    public static DemoSpinningCubeNode AddDemoNode(Vector3 pos)
    {
        var demoNode = new DemoSpinningCubeNode
        {
            Position = pos
        };
        return demoNode;
    }

    /// <summary>
    /// Starting point to change project settings
    /// </summary>
    /// <param name="projectSettings"></param>
    public static void LoadProjectSettings(ProjectSettings projectSettings)
    {
        ProjectSettings.SetSetting("application/config/name", "GodotApplication");
    }

    /// <summary>
    /// Main entry point for starting and using godot as a library
    /// </summary>
    /// <param name="scene"></param>
    public static void LoadScene(SceneTree scene) 
    {
        //Build root node to store the 3d scene on
        var rootNode = new Node3D();

        //Add camera to see the spinning demo cubes
        var camera = new Camera3D
        {
            Current = true,
            Position = new Vector3(0, 0, 2)
        };
        rootNode.AddChild(camera);

        //Attach 3 demo cubes where the camera can see them
        rootNode.AddChild(AddDemoNode(new Vector3(1, 1, 1)));
        rootNode.AddChild(AddDemoNode(new Vector3(-1, -1, -1)));
        rootNode.AddChild(AddDemoNode(new Vector3(0, 1, 1)));

        //Finally we need to add root node to the main window for rendering
        scene.Root.AddChild(rootNode);
    }
}