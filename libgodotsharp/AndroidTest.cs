using System;
using System.Collections.Generic;
using System.Text;

namespace LibGodotSharp
{
    public static class AndroidTest
    {
        static bool? isAndroid;
        public static bool Check()
        {
            if (isAndroid != null) return (bool)isAndroid;
            using var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "getprop";
            process.StartInfo.Arguments = "ro.build.user";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            try
            {
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                isAndroid = string.IsNullOrEmpty(output) ? (bool?)false : (bool?)true;
            }
            catch
            {
                isAndroid = false;
            }
            return (bool)isAndroid;
        }
    }
}
