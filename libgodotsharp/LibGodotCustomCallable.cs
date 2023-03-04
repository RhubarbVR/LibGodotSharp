using GDExtension;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static GDExtension.Native;
using static LibGodotSharp.LibGodotCustomCallable;

namespace LibGodotSharp
{
    internal static unsafe class LibGodotCustomCallable
    {
        [DllImport("godot_android", EntryPoint = "libgodot_create_callable", CallingConvention = CallingConvention.StdCall)]
        internal static extern void* Android_libgodot_create_callable(void* customobject);

        [DllImport("libgodot", EntryPoint = "libgodot_create_callable", CallingConvention = CallingConvention.StdCall)]
        internal static extern void* Desktop_libgodot_create_callable(void* customobject);


        [DllImport("godot_android", EntryPoint = "libgodot_bind_custom_callable", CallingConvention = CallingConvention.StdCall)]
        internal static extern void Android_libgodot_bind_custom_callable(void* callable_hash_bind, void* get_as_text_bind, void* get_object_bind, void* disposes_bind, void* call_bind);

        [DllImport("libgodot", EntryPoint = "libgodot_bind_custom_callable", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Desktop_libgodot_bind_custom_callable(void* callable_hash_bind, void* get_as_text_bind, void* get_object_bind, void* disposes_bind, void* call_bind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Libgodot_bind_custom_callable(void* callable_hash_bind, void* get_as_text_bind, void* get_object_bind, void* disposes_bind, void* call_bind)
        {
            if (AndroidTest.Check())
            {
                Android_libgodot_bind_custom_callable(callable_hash_bind, get_as_text_bind, get_object_bind, disposes_bind, call_bind);
            }
            else
            {
                Desktop_libgodot_bind_custom_callable(callable_hash_bind, get_as_text_bind, get_object_bind, disposes_bind, call_bind);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void* Libgodot_create_callable(void* customobject)
        {
            if (AndroidTest.Check())
            {
                return Android_libgodot_create_callable(customobject);
            }
            else
            {
                return Desktop_libgodot_create_callable(customobject);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate uint callable_hash_bind_del(void* targetObject);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void* get_as_text_bind_del(void* targetObject);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ulong get_object_bind_del(void* targetObject);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void disposes_bind_del(void* targetObject);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void call_bind_del(void* targetObject, void** args, int args_length, void* returnData, void* error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Callable GetCallable(void* customobject)
        {
            return (Callable)GCHandle.FromIntPtr(new IntPtr(customobject)).Target;
        }

        internal static uint Callable_hash_bind(void* targetObject)
        {
            var target = GetCallable(targetObject);
            return (uint)(target._delegate?.GetHashCode() ?? target.GetHashCode());
        }

        internal static void* Get_as_text_bind_del(void* targetObject)
        {
            var target = GetCallable(targetObject);
            return StringMarshall.ToNative(target._delegate?.ToString() ?? "null");
        }

        internal static ulong Get_object_bind_del(void* targetObject)
        {
            var target = GetCallable(targetObject);
            return (ulong)target._object.GetInstanceId();
        }

        internal static void Disposes_bind_del(void* targetObject)
        {
            GetCallable(targetObject).Free();
        }

        internal static void Call_bind_del(void* targetObject, void** args, int args_length, void* returnData, void* error)
        {
            var target = GetCallable(targetObject);
            var callargs = new object[args_length];
            for (int i = 0; i < args_length; i++)
            {
                var varenet = new Variant(args[i]);
                callargs[i] = Variant.VariantToObject(varenet);
            }
            var output = target._delegate.DynamicInvoke(callargs);
            Variant.SaveIntoPointer(output, returnData);
        }

        internal static void Init()
        {
            var callable_hash_pointer = (void*)SaftyRapper.GetFunctionPointerForDelegate(Callable_hash_bind);
            var get_as_text_pointer = (void*)SaftyRapper.GetFunctionPointerForDelegate(Get_as_text_bind_del);
            var get_object_pointer = (void*)SaftyRapper.GetFunctionPointerForDelegate(Get_object_bind_del);
            var disposes_pointer = (void*)SaftyRapper.GetFunctionPointerForDelegate(Disposes_bind_del);
            var call_pointer = (void*)SaftyRapper.GetFunctionPointerForDelegate(Call_bind_del);
            Libgodot_bind_custom_callable(callable_hash_pointer, get_as_text_pointer, get_object_pointer, disposes_pointer, call_pointer);
        }
    }
}
