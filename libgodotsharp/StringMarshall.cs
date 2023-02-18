using System;

namespace GDExtension;

public unsafe static class StringMarshall
{

    public static void* ToNative(string managed)
    {
        var x = GDExtensionMain.extensionInterface.mem_alloc(8);
        fixed (char* ptr = managed)
        {
            GDExtensionMain.extensionInterface.string_new_with_utf16_chars(x, (ushort*)ptr);
        }
        return x;
    }

    public static string ToManaged(void* str)
    {
        var l = (int)GDExtensionMain.extensionInterface.string_to_utf16_chars(str, null, 0);
        var span = stackalloc char[l];
        GDExtensionMain.extensionInterface.string_to_utf16_chars(str, (ushort*)span, l);
        return new string(span, 0, l);
    }
}
