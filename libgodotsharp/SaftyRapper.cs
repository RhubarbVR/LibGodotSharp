
namespace GDExtension;

public static class SaftyRapper
{
    public unsafe static TDelegate GetDelegateForFunctionPointer<TDelegate>(void* ptr) where TDelegate : Delegate
    {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>((IntPtr)ptr);
    }
    public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr ptr) where TDelegate : Delegate
    {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);
    }
    public static IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate ptr) where TDelegate : Delegate
    {
        return Marshal.GetFunctionPointerForDelegate(ptr);
    }
}
