using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpGenerator
{
    internal class Program
    {
        public static IntPtr GodotLibrary;
        public static string GodotRootDir;
        public static bool skipScons = false;
        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warn: " + message);
            Console.ResetColor();
        }

        [DllImport("libgodot")]
        public static extern int godot_main(int argc, string[] args);

        static void Main(string[] args)
        {
            GodotRootDir = Directory.GetCurrentDirectory();
            if (args.Length >= 1)
            {
                GodotRootDir = args[0];
            }
            var startingDir = GodotRootDir;
            if (!File.Exists(Path.Combine(GodotRootDir, "SConstruct")))
            {
                GodotRootDir = Path.Combine(startingDir, "..", "godot-lib");
            }
            if (!File.Exists(Path.Combine(GodotRootDir, "SConstruct")))
            {
                GodotRootDir = Path.Combine(startingDir, "..", "..", "..", "..", "..", "godot-lib");
            }
            if (!File.Exists(Path.Combine(GodotRootDir, "SConstruct")))
            {
                GodotRootDir = Path.Combine(startingDir, "..", "godot");
            }
            if (!File.Exists(Path.Combine(GodotRootDir, "SConstruct")))
            {
                GodotRootDir = Path.Combine(startingDir, "..", "..", "..", "..", "..", "godot");
            }
            if (!File.Exists(Path.Combine(GodotRootDir, "SConstruct")))
            {
                throw new Exception("Failed to find lib godot root");
            }
            Console.WriteLine($"Godot Root Dir:{GodotRootDir}");
            var path = Path.Combine(GodotRootDir, "bin");
            if (!skipScons)
            {
                if (!SconsRunner.RunScons())
                {
                    throw new Exception("Failed to run scons");
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => Path.Combine(path, $"godot.windows.editor.dev.x86_64.dll"),
                    Architecture.Arm64 => Path.Combine(path, $"godot.windows.editor.dev.arm64.dll"),
                    Architecture.Arm => Path.Combine(path, $"godot.windows.editor.dev.arm32.dll"),
                    _ => Path.Combine(path, $"godot.windows.editor.dev.x86_32.dll"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                path = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => Path.Combine(path, $"godot.macos.editor.dev.x86_64.dylib"),
                    Architecture.Arm64 => Path.Combine(path, $"godot.macos.editor.dev.arm64.dylib"),
                    Architecture.Arm => Path.Combine(path, $"godot.macos.editor.dev.arm32.dylib"),
                    _ => Path.Combine(path, $"godot.macos.editor.dev.x86_32.dylib"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                path = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => Path.Combine(path, $"godot.linuxbsd.editor.dev.x86_64.so"),
                    Architecture.Arm64 => Path.Combine(path, $"godot.linuxbsd.editor.dev.arm64.so"),
                    Architecture.Arm => Path.Combine(path, $"godot.linuxbsd.editor.dev.arm32.so"),
                    _ => Path.Combine(path, $"godot.linuxbsd.editor.dev.x86_32.so"),
                };
            }
            if (!File.Exists(path))
            {
                throw new Exception("Editor build not found");
            }
            try
            {
                var copyfile = Path.Combine(path, "..", "libgodot");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    copyfile += ".dll";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    copyfile += ".dylib";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    copyfile += ".so";
                }
                File.Copy(path, copyfile, true);
                if (!File.Exists(copyfile))
                {
                    throw new Exception($"Editor file copy failed from {path} to {copyfile}");
                }
                var GodotLibrary = NativeLibrary.Load(copyfile);
                if (GodotLibrary == IntPtr.Zero)
                {
                    throw new Exception("Failed to laod godot");
                }
                if (godot_main(2, new string[] { "libgodot", "--dump-extension-api" ,"-v" }) != 0)
                {
                    throw new Exception("Godot had error");
                }
            }
            catch (Exception e)
            {
                Warn(e.ToString());
            }
            var pathToGenJson = Path.Combine(Environment.CurrentDirectory, "extension_api.json");
            if (!File.Exists(pathToGenJson))
            {
                pathToGenJson = Path.Combine(Directory.GetCurrentDirectory(), "extension_api.json");
            }
            if (!File.Exists(pathToGenJson))
            {
                throw new Exception("Failed to find extension_api json");
            }
            var ginDirParent = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "libgodotsharp");
            if (!Directory.Exists(ginDirParent))
            {
                ginDirParent = Path.Combine(Directory.GetCurrentDirectory(), "libgodotsharp");
            }
            if (!Directory.Exists(ginDirParent))
            {
                ginDirParent = Path.Combine(Directory.GetCurrentDirectory(), "..", "libgodotsharp");
            }
            if (!Directory.Exists(Path.Combine(ginDirParent, "Extensions")))
            {
                throw new Exception("Don't know where to put files");
            }
            var ginDir = Path.Combine(ginDirParent, "Generated");
            if (Directory.Exists(ginDir))
            {
                Directory.Delete(ginDir, true);
            }
            Directory.CreateDirectory(ginDir);
            var docs = Path.Combine(GodotRootDir, "doc", "classes") + "/";
            var configName = "float_32";
            var api = Api.Create(pathToGenJson);
            var convert = new Convert(api, ginDir, docs, configName);
            convert.Emit();
        }
    }
}
