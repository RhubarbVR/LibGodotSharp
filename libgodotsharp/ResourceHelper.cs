using System.Reflection;

namespace GDExtension
{
    public static class ResourceHelper
    {
        public static byte[] GetResourceBytes(string path)
        {
            var targetAsm = Assembly.GetCallingAssembly();
            using var mem = new MemoryStream();
            targetAsm.GetManifestResourceStream(path).CopyTo(mem);
            return mem.ToArray();
        }

        public static string[] GetResourceNames()
        {
            return Assembly.GetCallingAssembly().GetManifestResourceNames();
        }

        public static ManifestResourceInfo GetResourceInfo(string path)
        {
            return Assembly.GetCallingAssembly().GetManifestResourceInfo(path);
        }

    }
}
