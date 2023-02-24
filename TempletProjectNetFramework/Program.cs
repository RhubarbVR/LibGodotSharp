
using LibGodotSharp;

internal class Program
{
    static unsafe int Main(string[] args)
    {
        var runVerbose = false;
#if DEBUG
        runVerbose = true;
#endif
        //Passes arguments down to godot
        return LibGodotManager.RunGodot(args, ExtensionEntry.EntryPoint, GodotApplication.LoadScene, runVerbose);
    }
}
