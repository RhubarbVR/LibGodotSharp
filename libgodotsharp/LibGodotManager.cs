using GDExtension;
using System.IO;
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
                libgodot_bind(SaftyRapper.GetFunctionPointerForDelegate(entryPoint), SaftyRapper.GetFunctionPointerForDelegate<SceneTreeLoadNative>(RunStartUp));
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
