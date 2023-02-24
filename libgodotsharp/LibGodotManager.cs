using GDExtension;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using static GDExtension.Native;

namespace LibGodotSharp
{
    public static unsafe class LibGodotManager
    {
        public delegate void SceneTreeLoad(SceneTree scene);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SceneTreeLoadNative(void* scene);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool GDentryPoint(GDExtensionInterface interface_, void* library, GDExtensionInitialization* expIntilzation);

        [DllImport("godot_android", EntryPoint = "libgodot_bind", CallingConvention = CallingConvention.StdCall)]
        internal static extern void Android_libgodot_bind(IntPtr entryPoint, IntPtr sceneTreeLoad);

        [DllImport("libgodot", CallingConvention = CallingConvention.StdCall)]
        internal static extern void libgodot_bind(IntPtr entryPoint, IntPtr sceneTreeLoad);

        [DllImport("libgodot", CallingConvention = CallingConvention.StdCall)]
        internal static extern int godot_main(int amount, string[] args);
        internal static SceneTreeLoad _sceneTreeLoad;
        internal static void RunStartUp(void* startup)
        {
            _sceneTreeLoad(SceneTree.Construct(startup));
        }

        private static void FallBackLoadPlatformLibrary()
        {
            var neededPlatfomFile = "Unknown";
            var arch = RuntimeInformation.OSArchitecture.ToString().ToLower();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                neededPlatfomFile = Path.Combine($"win-{arch}", "native", "libgodot.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                neededPlatfomFile = Path.Combine($"linux-{arch}", "native", "libgodot.so");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                neededPlatfomFile = Path.Combine($"osx-{arch}", "native", "libgodot.dylib");
            }
            else
            {
                neededPlatfomFile = Path.Combine($"{RuntimeInformation.OSDescription.ToLower()}-{arch}", "native", "libgodot.so");
            }
            var runtimesFolder = Path.Combine(Directory.GetCurrentDirectory(), "runtimes", neededPlatfomFile);
            if (!File.Exists(runtimesFolder))
            {
                runtimesFolder = Path.Combine(Assembly.GetEntryAssembly().Location, "runtimes", neededPlatfomFile);
            }
            if (!File.Exists(runtimesFolder))
            {
                runtimesFolder = Path.Combine(Assembly.GetAssembly(typeof(LibGodotManager)).Location, "runtimes", neededPlatfomFile);
            }
            if (!File.Exists(runtimesFolder))
            {
                throw new DllNotFoundException($"Failed to find libgodot for {RuntimeInformation.OSDescription.ToLower()} arc:{RuntimeInformation.ProcessArchitecture.ToString().ToLower()}");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    LoadLibraryWindows(runtimesFolder, arch);
                }
                catch
                {
                    Console.WriteLine($"Failed to load library natively Target:{runtimesFolder} now lazy loading");
                    LazyLoadLibrary(runtimesFolder);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    LoadLibraryLinux(runtimesFolder, arch);
                }
                catch
                {
                    Console.WriteLine($"Failed to load library natively Target:{runtimesFolder} now lazy loading");
                    LazyLoadLibrary(runtimesFolder);
                }
            }
            else
            {
                LazyLoadLibrary(runtimesFolder);
            }
        }
        private static void LazyLoadLibrary(string library)
        {
            var ex = Path.GetExtension(library);
            File.Copy(library, Path.Combine(Assembly.GetEntryAssembly().Location, "libgodot" + ex), true);
        }

        [DllImport("libdl", CharSet = CharSet.Unicode)]
        static extern IntPtr dlopen(string fileName, int flags);

        private static void LoadLibraryLinux(string library, string arch)
        {
            const int RTLD_NOW = 2;
            if (!(
                   dlopen(library, RTLD_NOW) != IntPtr.Zero
                || dlopen($"./runtimes/linux-{arch}/native/libgodot.so", RTLD_NOW) != IntPtr.Zero
                || dlopen("libgodot.so", RTLD_NOW) != IntPtr.Zero))
            {
                throw new DllNotFoundException(library);
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibraryW(string fileName);

        private static void LoadLibraryWindows(string library, string arch)
        {
            if (!(
                    LoadLibraryW(library) != IntPtr.Zero
                 || LoadLibraryW($"runtimes/win-{arch}/native/libgodot.dll") != IntPtr.Zero
                 || LoadLibraryW("libgodot") != IntPtr.Zero)
               )
            {
                throw new DllNotFoundException(library);
            }
        }

        public static int RunGodot(string[] args, GDentryPoint entryPoint, SceneTreeLoad sceneTreeLoad, bool verboes = false)
        {
            if (sceneTreeLoad is null)
            {
                throw new Exception("Needs sceneTreeLoad");
            }
            if (_sceneTreeLoad is not null)
            {
                throw new Exception("All ready bound into godot");
            }
            _sceneTreeLoad = sceneTreeLoad;
            if (AndroidTest.Check())
            {
                Android_libgodot_bind(SaftyRapper.GetFunctionPointerForDelegate(entryPoint), SaftyRapper.GetFunctionPointerForDelegate<SceneTreeLoadNative>(RunStartUp));
                return 0;
            }
            else
            {
                try
                {
                    libgodot_bind(SaftyRapper.GetFunctionPointerForDelegate(entryPoint), SaftyRapper.GetFunctionPointerForDelegate<SceneTreeLoadNative>(RunStartUp));
                }
                catch (DllNotFoundException)
                {
                    FallBackLoadPlatformLibrary();
                    libgodot_bind(SaftyRapper.GetFunctionPointerForDelegate(entryPoint), SaftyRapper.GetFunctionPointerForDelegate<SceneTreeLoadNative>(RunStartUp));
                }
            }
            var argss = new List<string>(args);
            argss.Insert(0, "libgodot");
            if (verboes)
            {
                argss.Add("--verbose");
            }
            return godot_main(argss.Count, argss.ToArray());
        }

    }
}
