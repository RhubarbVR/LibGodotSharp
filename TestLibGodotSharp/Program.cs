using GDExtension;
using LibGodotSharp;
namespace TestLibGodotSharp
{
    internal class Program
    {
        static unsafe int Main(string[] args)
        {
            return LibGodotManager.RunGodot(args, ExtensionEntry.EntryPoint, MainGodotGame.LoadScene, true);
        }
    }
}