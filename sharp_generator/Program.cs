using System.Diagnostics;
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
            Console.WriteLine($"Working Dir {Directory.GetCurrentDirectory()}");
            GodotRootDir = Directory.GetCurrentDirectory();
            if (args.Length >= 1)
            {
                GodotRootDir = args[0];
            }
            var startingDir = GodotRootDir;
            if (!File.Exists(Path.Combine(GodotRootDir, "SConstruct")))
            {
                GodotRootDir = Path.Combine(startingDir, "godot");
                Warn("thinking it is running in github action so run scons library_type=shared_library");
                skipScons = true;
            }
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
                    Architecture.X64 => Path.Combine(path, $"godot.windows.editor.x86_64.dll"),
                    Architecture.Arm64 => Path.Combine(path, $"godot.windows.editor.arm64.dll"),
                    Architecture.Arm => Path.Combine(path, $"godot.windows.editor.arm32.dll"),
                    _ => Path.Combine(path, $"godot.windows.editor.x86_32.dll"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                path = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => Path.Combine(path, $"godot.macos.editor.x86_64.dylib"),
                    Architecture.Arm64 => Path.Combine(path, $"godot.macos.editor.arm64.dylib"),
                    Architecture.Arm => Path.Combine(path, $"godot.macos.editor.arm32.dylib"),
                    _ => Path.Combine(path, $"godot.macos.editor.x86_32.dylib"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                path = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => Path.Combine(path, $"godot.linuxbsd.editor.x86_64.so"),
                    Architecture.Arm64 => Path.Combine(path, $"godot.linuxbsd.editor.arm64.so"),
                    Architecture.Arm => Path.Combine(path, $"godot.linuxbsd.editor.arm32.so"),
                    _ => Path.Combine(path, $"godot.linuxbsd.editor.x86_32.so"),
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
                GodotLibrary = NativeLibrary.Load(copyfile);
                if (GodotLibrary == IntPtr.Zero)
                {
                    throw new Exception("Failed to laod godot");
                }
                var custom_args = new string[] { "libgodot", "--dump-extension-api", "--verbose", "--headless", "" };
                if (godot_main(custom_args.Length - 1, custom_args) != 0)
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

            //Copy all platform files

            CopyFileWithDirectory("godot-lib.template_release.aar", "./LibGodotSharpAndroid/godot-lib.template_release.aar");
            CopyFileWithDirectory("godot.windows.template_release.x86_32.dll", "./LibGodotSharpDesktop/runtimes/win-x86/native/libgodot.dll");
            CopyFileWithDirectory("godot.windows.template_release.x86_64.dll", "./LibGodotSharpDesktop/runtimes/win-x64/native/libgodot.dll");
            CopyFileWithDirectory("libgodot.macos.template_release.arm64.dylib", "./LibGodotSharpDesktop/runtimes/osx-arm64/native/libgodot.dylib");
            CopyFileWithDirectory("libgodot.macos.template_release.x86_64.dylib", "./LibGodotSharpDesktop/runtimes/osx-x64/native/libgodot.dylib");
            CopyFileWithDirectory("libgodot.linuxbsd.template_release.x86_64.so", "./LibGodotSharpDesktop/runtimes/linux-x64/native/libgodot.so");
        }

        public static void CopyFileWithDirectory(string sourceFilePath, string destFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                Warn($"Did not find {sourceFilePath}");
                return;
            }
            // Ensure the directory exists
            string destDirectory = Path.GetDirectoryName(destFilePath);
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            // Copy the file
            File.Copy(sourceFilePath, destFilePath, true);
        }
    }
}
