using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpGenerator
{
    internal class Program
    {
        public static IntPtr GodotLibrary;
        public static string GodotRootDir;
        public static bool skipScons = false;
        public static string GithubBuildVersion = null;
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

            GithubBuildVersion = Environment.GetEnvironmentVariable("BUILD_VERSION");
            if (GithubBuildVersion is null)
            {
                return;
            }
            CopyFileWithDirectory("godot-lib.template_release.aar", "./LibGodotSharpAndroid/godot-lib.template_release.aar");
            CopyFileWithDirectory("godot.windows.template_release.x86_32.dll", "./LibGodotSharpDesktop/runtimes/win-x86/native/libgodot.dll");
            CopyFileWithDirectory("godot.windows.template_release.x86_64.dll", "./LibGodotSharpDesktop/runtimes/win-x64/native/libgodot.dll");
            CopyFileWithDirectory("libgodot.macos.template_release.arm64.dylib", "./LibGodotSharpDesktop/runtimes/osx-arm64/native/libgodot.dylib");
            CopyFileWithDirectory("libgodot.macos.template_release.x86_64.dylib", "./LibGodotSharpDesktop/runtimes/osx-x64/native/libgodot.dylib");
            CopyFileWithDirectory("libgodot.linuxbsd.template_release.x86_64.so", "./LibGodotSharpDesktop/runtimes/linux-x64/native/libgodot.so");

            // Setup templet project
            var templetDir = Path.Combine(Directory.GetCurrentDirectory(), "TempletProject");

            var removeOnBuild = @"Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""REMOVEONBUILD"", ""REMOVEONBUILD"", ""{2D13262A-4BF2-45B0-92CF-3203C4F46A95}""
EndProject
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""LibGodotSharp"", ""..\libgodotsharp\LibGodotSharp.csproj"", ""{95BE9533-B6BA-415C-B548-B3A2C734196D}""
EndProject
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""LibGodotSharpAndroid"", ""..\LibGodotSharpAndroid\LibGodotSharpAndroid.csproj"", ""{7258ECD1-63B8-4691-B055-7DE1DC7F5740}""
EndProject
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""LibGodotSharpDesktop"", ""..\LibGodotSharpDesktop\LibGodotSharpDesktop.csproj"", ""{8D862275-156A-4379-BE02-8CDFA01DF2CA}""
EndProject";
            ReplaceTextInFile(Path.Combine(templetDir, "GodotApplication.sln"), removeOnBuild, null);
            ReplaceTextInFile(Path.Combine(templetDir, "GodotApplication", "GodotApplication.csproj"), "<ProjectReference Include=\"..\\..\\libgodotsharp\\LibGodotSharp.csproj\" OutputItemType=\"Analyzer\" ReferenceOutputAssembly=\"true\" />", $"<PackageReference Include=\"LibGodotSharp\" Version=\"{GithubBuildVersion}\" />");
            ReplaceTextInFile(Path.Combine(templetDir, "Platforms", "Desktop", "DesktopPlatform.csproj"), "<ProjectReference Include=\"..\\..\\..\\LibGodotSharpDesktop\\LibGodotSharpDesktop.csproj\" />", $"<PackageReference Include=\"LibGodotSharp.Desktop\" Version=\"{GithubBuildVersion}\" />");
            ReplaceTextInFile(Path.Combine(templetDir, "Platforms", "Android", "AndroidPlatform.csproj"), "<ProjectReference Include=\"..\\..\\..\\LibGodotSharpAndroid\\LibGodotSharpAndroid.csproj\" />", $"<PackageReference Include=\"LibGodotSharp.Android\" Version=\"{GithubBuildVersion}\" />");

            Console.WriteLine("Done setting up TempletProject");
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
                file.Close();
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }

        public static void ReplaceTextInFile(string file, string search, string replace)
        {
            var encode = GetEncoding(file);
            File.WriteAllText(file, File.ReadAllText(file).Replace(search, replace), encode);
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
