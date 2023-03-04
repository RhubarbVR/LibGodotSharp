using LibGodotSharp;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace GDExtension;

public unsafe partial class Callable
{
    protected Callable(Delegate @delegate,Object @object)
    {
        if (@delegate is null)
        {
            throw new NullReferenceException();
        }
        _delegate = @delegate;
        _object = @object;
        gCHandle = GCHandle.Alloc(this);
        _internal_pointer = LibGodotCustomCallable.Libgodot_create_callable((void*)(IntPtr)gCHandle);
    }
    internal readonly GCHandle gCHandle;
    internal Delegate _delegate;
    internal Object _object;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Callable From<T>(T target, Object @object = null) where T : Delegate
    {
        if(target is null)
        {
            throw new NullReferenceException();
        }
        if(target.Target is Object delobject)
        {
            @object = delobject;
        }
        if(@object is null)
        {
            throw new NullReferenceException();
        }
        return new Callable(target, @object);
    }

    internal void Free()
    {
        gCHandle.Free();
        _delegate = null;
        _object = null;
    }
}