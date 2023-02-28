
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace GDExtension;

public static class SaftyRapper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static TDelegate GetDelegateForFunctionPointer<TDelegate>(void* ptr) where TDelegate : Delegate
    {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>((IntPtr)ptr);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr ptr) where TDelegate : Delegate
    {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate ptr) where TDelegate : Delegate
    {
        return Marshal.GetFunctionPointerForDelegate(ptr);
    }
}
