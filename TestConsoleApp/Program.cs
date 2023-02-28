using GodotGame;
using LibGodotSharp;

namespace TestConsoleApp
{
    internal class Program
    {
        static unsafe int Main(string[] args)
        {
            return LibGodotManager.RunGodot(Array.Empty<string>(), TestLibGodotSharpExtensionEntry.EntryPoint, MainGodotGame.LoadScene, MainGodotGame.LoadProjectSettings, true);
        }
    }
}