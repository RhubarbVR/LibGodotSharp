using GodotGame;
using LibGodotSharp;

namespace TestConsoleApp
{
    internal class Program
    {
        static unsafe int Main(string[] args)
        {
            return LibGodotManager.RunGodot(Array.Empty<string>(), ExtensionEntry.EntryPoint, MainGodotGame.LoadScene, MainGodotGame.LoadProjectSettings, true);
        }
    }
}